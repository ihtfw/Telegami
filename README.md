# Telegami

A simple library for building Telegram bots.

The goal of Telegami is to keep things straightforward and easy to use.  
It builds on top of the low-level [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) library, which handles communication with the Telegram Bot API.

There’s also the [TelegramBotFramework](https://github.com/MajMcCloud/TelegramBotFramework), but it's a bit too complex for many common use cases.

Inspired by the Node.js [Telegraf](https://github.com/telegraf/telegraf) library, Telegami aims to offer a clear and intuitive experience for handling basic bot scenarios.

**Current status:** Active development — breaking changes may occur between releases.

# Architecture

- Uses `Microsoft.Extensions.DependencyInjection` for dependency injection, following the .NET standard.
- Each message is handled in a new DI scope — similar to the per-request model in ASP.NET.
- The syntax is inspired by the Telegraf library and combines well with ASP.NET minimal APIs, allowing you to inject dependencies directly into delegates.
- Only one handler will process each incoming message — the **first** one whose condition matches.
- Handlers are evaluated in the order they are added to the bot.


# Basic Usage Examples

## Getting Started

```csharp
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
// Register your services here if needed

var serviceProvider = serviceCollection.BuildServiceProvider();

var bot = new TelegamiBot(serviceProvider, "BOT_TOKEN_FROM_BOT_FATHER");

// Handle /start command
bot.Start(async ctx =>
{
    await ctx.ReplyAsync("Hello! I'm a bot. How can I help you?");
});

// Handle /custom command
bot.Command("custom", async ctx =>
{
    await ctx.ReplyAsync($"This is a custom command handler. Arguments: '{ctx.BotCommand!.Arguments}'");
});

await bot.LaunchAsync();
```

That's it! Bot is working and can handle **/start** and **/custom** commands.

## Specific message type

```CSharp
// response only on Sticker message type
bot.On(MessageType.Sticker, async ctx => { await ctx.ReplyAsync($"What a nice sticker!"); });

// response only on Text message type
bot.On(MessageType.Text, async ctx => { await ctx.ReplyAsync("Echo: " + ctx.Message.Text); });

// response to any message
bot.On(async ctx => { await ctx.ReplyAsync("Type: " + ctx.Message.Type); });
```

## Dependecy injection

```CSharp
class MyCustomService{}

var serviceCollection = new ServiceCollection();
// register service
serviceCollection.AddScoped<MyCustomService>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var bot = new TelegamiBot(serviceProvider, "BOT_TOKEN_FROM_BOT_FATHER");

// ask for it!
bot.Command("dependency", async (MessageContext ctx, MyCustomService myCustomService)  =>
{
    await ctx.ReplyAsync($"This is a dependency injection handler for service: " + myCustomService.GetType().Name);
});

// .. other code
```

# Advanced scenarios

Often is is neccecary to interact with the user and not just process command, i.e. collect name, last name and age.
That's why we have Scene. Scene is bounded to user id, chat id and thread id, that means that different users can interect and each one will have it's one data. Also when user is in Scene, all messages are handled by scene, not by bot.

Let's check out 2 appoaches Scene and WizardScene.

## Scene

Easy to use and it's like bot inside bot with special handlers Enter and Leave.

```CSharp

// let's add command, so we can start scene
bot.Command("person", async ctx => { await ctx.EnterSceneAsync("person_scene"); });

// let's define scene, where we will collect all required information
var personCardScene = new Scene("person_scene");
// Enter will be executed when EnterSceneAsync is invoked with message context that was at that moment
personCardScene.Enter(async ctx => await ctx.SendAsync("Hi! What's your name?"));

// Leave will be executed when LeaveSceneAsync is invoked with message context that was at that moment
personCardScene.Leave(async ctx =>
{
    var person = ctx.Session.Get<Person>() ?? new Person();

    await ctx.ReplyAsync($"Your name is {person.Name} {person.LastName}, you are {person.Age} years old.");
});

// 
personCardScene.On(MessageType.Text, async ctx =>
{
    var person = ctx.Session.Get<Person>() ?? new Person();

    // get name
    if (string.IsNullOrEmpty(person.Name))
    {
        person.Name = ctx.Message.Text;
        ctx.Session.Set(person);
        await ctx.ReplyAsync($"What's your last name?");
        return;
    }

    // get last name
    if (string.IsNullOrEmpty(person.LastName))
    {
        person.LastName = ctx.Message.Text;
        ctx.Session.Set(person);
        await ctx.ReplyAsync($"What's your age?");
        return;
    }

    // age
    if (!int.TryParse(ctx.Message.Text, out var age))
    {
        await ctx.ReplyAsync($"Age should be a number!");
        return;
    }

    person.Age = age;
    // and leave!, so message handling is returned to regular bot
    await ctx.LeaveSceneAsync();
});

// also it's required to add scene to bot, so it knows about it
bot.AddScene(personCardScene);

```

## WizardScene

Have stages, that can be invoked one by one. Let's see same example with this approach.
For wizard we have special class WizardContext to track out wizard step and WizardContext<TState> to track step and also have strongly typed state.

```CSharp

// let's add command, so we can start scene
bot.Command("person_wizard", async ctx => { await ctx.EnterSceneAsync("person_wizard_scene"); });

var wizardScene = new WizardScene("person_wizard_scene",
    async (MessageContext ctx, WizardContext wiz) =>
    {
        // first step will be invoked on Enter
        await ctx.SendAsync("Hi! What's your name?");
        // now we change step and next message will be processed with it
        wiz.Next();
    },
    async (MessageContext ctx, WizardContext<Person> wiz) =>
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
            // we didn't call Next(), so next message will be handled by this step again
            await ctx.SendAsync("Incorrect message, please send text");
            return;
        }
        // set name, that all, state will be saved automatically
        wiz.State.Name = ctx.Message.Text;

        await ctx.SendAsync("Hi! What's your last name?");
        wiz.Next();
    },
    async (MessageContext ctx, WizardContext<Person> wiz) =>
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
            // we didn't call Next(), so next message will be handled by this step again
            await ctx.SendAsync("Incorrect message, please send text");
            return;
        }

        wiz.State.LastName = ctx.Message.Text;

        await ctx.SendAsync("Hi! What's your age?");
        wiz.Next();
    },
    async (MessageContext ctx, WizardContext<Person> wiz) =>
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
            await ctx.SendAsync("Incorrect message, please send text");
            return;
        }

        if (!int.TryParse(ctx.Message.Text, out var age))
        {
            await ctx.SendAsync("Age should be a number!");
            return;
        }

        wiz.State.Age = age;

        await ctx.SendAsync($"Thank you! Your information is:\n{wiz.State}");
        // and leave scene at the end
        await ctx.LeaveSceneAsync();
    }
);

bot.AddScene(wizardScene);

```

# Middleware

Inspired by ASP.NET

## Error handler example

Same logic as in ASP.NET every middleware have next delegate, so we can process MessageContext.

```CSharp

//.. other code

var bot = new TelegamiBot(serviceProvider, token);
bot.Use<GlobalErrorHandlerMiddleware>();

//.. other code

await bot.LaunchAsync();

class GlobalErrorHandlerMiddleware : ITelegamiMiddleware
{
    public async Task InvokeAsync(MessageContext ctx, MessageContextDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            await ctx.ReplyAsync("error: " + e.Message);
        }
    }
}
```