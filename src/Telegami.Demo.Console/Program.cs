using Microsoft.Extensions.DependencyInjection;
using Telegami;
using Telegami.Demo.Console;
using Telegami.Demo.Console.Commands;
using Telegami.Demo.Console.Middlewares;
using Telegami.Extensions;
using Telegami.Scenes;
using Telegami.Sessions;
using Telegram.Bot.Types.Enums;

var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("Please set the BOT_TOKEN environment variable!");
    return;
}

var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<MyCustomService>();
serviceCollection
    .AddTelegamiBot("demo", config =>
    {
        config.Token = token;
    })
    .AddCommands(typeof(Program))
    .AddMiddlewares(typeof(Program));

var serviceProvider = serviceCollection.BuildServiceProvider();

var botsManager = serviceProvider.GetRequiredService<TelegamiBotsManager>();
var bot = botsManager.Get("demo");

bot.Use<LoggerMiddleware>();
bot.Use<GlobalErrorHandlerMiddleware>();

bot.Start(async (MessageContext ctx, MyCustomService _) =>
{
    await ctx.ReplyAsync("Hello! I'm a bot. How can I help you?");
});

bot.Help(async ctx => { await ctx.ReplyAsync("This is a help message."); });

bot.Settings(async ctx => { await ctx.ReplyAsync("This is a settings message."); });

bot.Command<ClassCommandHandler>("class");

bot.Command("custom",
    async (MessageContext ctx) =>
    {
        await ctx.ReplyAsync($"this is custom command handler. args was: '{ctx.BotCommand!.Arguments}'");
    });

bot.Command("echo", async ctx => { await ctx.EnterSceneAsync("echo"); });

bot.Command("person", async ctx => { await ctx.EnterSceneAsync("person_scene"); });

bot.Command("person_wizard", async ctx => { await ctx.EnterSceneAsync("person_wizard_scene"); });

bot.Command("error", () => throw new Exception("This is a test exception"));

bot.On(MessageType.Sticker, async (MessageContext ctx) => { await ctx.ReplyAsync($"What a nice sticker!"); });

bot.Hears("hello", async (MessageContext ctx, MyCustomService _) => { await ctx.ReplyAsync("World!"); });

bot.Hears("world", async ctx => { await ctx.ReplyAsync("hello!"); });


#region echo scene

var echoScene = new Scene("echo");
echoScene.Enter(async ctx => await ctx.ReplyAsync("echo scene"));
echoScene.Leave(async ctx => await ctx.ReplyAsync("exiting echo scene"));
echoScene.Command("back", async ctx => await ctx.LeaveSceneAsync());
echoScene.Command("leave", async ctx => await ctx.LeaveSceneAsync());
echoScene.On(MessageType.Text, async ctx => await ctx.ReplyAsync(ctx.Message.Text ?? ""));
echoScene.On(async ctx => await ctx.ReplyAsync("Only text messages please"));
bot.AddScene(echoScene);

#endregion

#region person scene

var personCardScene = new Scene("person_scene");
personCardScene.Enter(async ctx => await ctx.SendAsync("Hi! What's your name?"));
personCardScene.Leave(async ctx =>
{
    var person = ctx.Session.Get<Person>() ?? new Person();

    await ctx.ReplyAsync($"Your name is {person.Name} {person.LastName}, you are {person.Age} years old.");
});

personCardScene.On(MessageType.Text, async ctx =>
{
    var person = ctx.Session.Get<Person>() ?? new Person();

    if (string.IsNullOrEmpty(person.Name))
    {
        person.Name = ctx.Message.Text;
        ctx.Session.Set(person);
        await ctx.ReplyAsync($"What's your last name?");
        return;
    }

    if (string.IsNullOrEmpty(person.LastName))
    {
        person.LastName = ctx.Message.Text;
        ctx.Session.Set(person);
        await ctx.ReplyAsync($"What's your age?");
        return;
    }

    if (!int.TryParse(ctx.Message.Text, out var age))
    {
        await ctx.ReplyAsync($"Age should be a number!");
        return;
    }

    person.Age = age;
    await ctx.LeaveSceneAsync();
});
bot.AddScene(personCardScene);

#endregion

#region wizard scene

var wizardScene = new WizardScene("person_wizard_scene",
    async (MessageContext ctx, WizardContext wiz) =>
    {
        await ctx.SendAsync("Hi! What's your name?");
        wiz.Next();
    },
    async (MessageContext ctx, WizardContext<Person> wiz) =>
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
            await ctx.SendAsync("Incorrect message, please send text");
            return;
        }
        wiz.State.Name = ctx.Message.Text;

        await ctx.SendAsync("Hi! What's your last name?");
        wiz.Next();
    },
    async (MessageContext ctx, WizardContext<Person> wiz) =>
    {
        if (string.IsNullOrEmpty(ctx.Message.Text))
        {
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
        await ctx.LeaveSceneAsync();
    }
);

bot.AddScene(wizardScene);

#endregion

#region nested scenes

bot.Command("root_scene", async ctx => await ctx.EnterSceneAsync("root_scene"));
var rootScene = new Scene("root_scene");
rootScene.Enter(async ctx => await ctx.ReplyAsync("This is root scene"));
rootScene.Leave(async ctx => await ctx.ReplyAsync("Exiting root scene"));
rootScene.Command("child", async ctx => await ctx.EnterSceneAsync("child_scene"));
rootScene.Command("back", async ctx => await ctx.LeaveSceneAsync());
rootScene.Command("leave", async ctx => await ctx.LeaveSceneAsync());
rootScene.Command("exit", async ctx => await ctx.LeaveSceneAsync());
rootScene.Command("ping", async ctx => await ctx.ReplyAsync("pong from root"));

var childScene = new Scene("child_scene");
childScene.Enter(async ctx => await ctx.ReplyAsync("This is child scene"));
childScene.Leave(async ctx => await ctx.ReplyAsync("Exiting child scene"));
childScene.Command("back", async ctx => await ctx.LeaveSceneAsync());
childScene.Command("ping", async ctx => await ctx.ReplyAsync("pong from child"));

bot.AddScene(rootScene);
bot.AddScene(childScene);

#endregion

bot.On(async ctx => { await ctx.ReplyAsync("not handled message"); });

Console.WriteLine("Launching bots...");

await botsManager.LaunchAsync();

Console.WriteLine("Bot is running... Press any key to exit.");
Console.ReadKey();