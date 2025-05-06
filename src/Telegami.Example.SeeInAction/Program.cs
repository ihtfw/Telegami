using Microsoft.Extensions.DependencyInjection;
using Telegami;
using Telegami.Example.SeeInAction;
using Telegami.Example.SeeInAction.Commands;
using Telegami.Example.SeeInAction.Middlewares;
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
    .AddTelegamiBot(token)
    .AddCommands(typeof(Program))
    .AddMiddlewares(typeof(Program));

var serviceProvider = serviceCollection.BuildServiceProvider();

var botsManager = serviceProvider.GetRequiredService<TelegamiBotsManager>();
var bot = botsManager.Get();

bot.Use<LoggerMiddleware>();
bot.Use<GlobalErrorHandlerMiddleware>();

// /start command
bot.Start(async (MessageContext ctx, MyCustomService _) =>
{
    await ctx.ReplyAsync("Hello! I'm a bot. How can I help you?");
});

// /help command
bot.Help(async ctx => { await ctx.ReplyAsync("This is a help message."); });

// /settings command
bot.Settings(async ctx => { await ctx.ReplyAsync("This is a settings message."); });

// /custom command
bot.Command("custom",
    async (MessageContext ctx) =>
    {
        await ctx.ReplyAsync($"this is custom command handler. args was: '{ctx.BotCommand!.Arguments}'");
    });

// /class command with handler moved to separate class implementation 
bot.Command<ClassCommandHandler>("class");

// /error command to check exception handling
bot.Command("error", () => throw new Exception("This is a test exception"));

bot.Command("echo", async ctx => { await ctx.EnterSceneAsync("echo"); });

bot.Command("person", async ctx => { await ctx.EnterSceneAsync("person_scene"); });

bot.Command("person_wizard", async ctx => { await ctx.EnterSceneAsync("person_wizard_scene"); });

// Handle only specific message types
bot.On(MessageType.Sticker, async (MessageContext ctx) => { await ctx.ReplyAsync($"What a nice sticker!"); });

// Common use case when we need to trigger on Text.Contains
bot.Hears("hello", async (MessageContext ctx, MyCustomService _) => { await ctx.ReplyAsync("World!"); });
bot.Hears("world", async ctx => { await ctx.ReplyAsync("hello!"); });

#region echo scene

bot.AddScene("echo", new Scene()
    .Enter(async ctx => await ctx.ReplyAsync("echo scene"))
    .Leave(async ctx => await ctx.ReplyAsync("exiting echo scene"))
    .Command("back", async ctx => await ctx.LeaveSceneAsync())
    .Command("leave", async ctx => await ctx.LeaveSceneAsync())
    .On(MessageType.Text, async ctx => await ctx.ReplyAsync(ctx.Message.Text ?? ""))
    .On(async ctx => await ctx.ReplyAsync("Only text messages please"))
);

#endregion

#region person scene

bot.AddScene("person_scene",
    new Scene()
        .Enter(async ctx => await ctx.SendAsync("Hi! What's your name?"))
        .Leave(async ctx =>
        {
            var person = ctx.Session.Get<Person>();

            await ctx.ReplyAsync($"Your name is {person.Name} {person.LastName}, you are {person.Age} years old.");
        })
        .On(MessageType.Text, async ctx =>
        {
            var person = ctx.Session.Get<Person>();

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
        }));

#endregion

#region wizard scene

bot.AddScene("person_wizard_scene", new WizardScene(async (MessageContext ctx, WizardContext wiz) =>
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
));

#endregion

#region nested scenes

bot.Command("root_scene", async ctx => await ctx.EnterSceneAsync("root_scene"));

bot.AddScene("root_scene", new Scene()
    .Enter(async ctx => await ctx.ReplyAsync("This is root scene"))
    .Leave(async ctx => await ctx.ReplyAsync("Exiting root scene"))
    .Command("child", async ctx => await ctx.EnterSceneAsync("child_scene"))
    .Command("back", async ctx => await ctx.LeaveSceneAsync())
    .Command("leave", async ctx => await ctx.LeaveSceneAsync())
    .Command("exit", async ctx => await ctx.LeaveSceneAsync())
    .Command("ping", async ctx => await ctx.ReplyAsync("pong from root"))
);
bot.AddScene("child_scene", new Scene()
    .Enter(async ctx => await ctx.ReplyAsync("This is child scene"))
    .Leave(async ctx => await ctx.ReplyAsync("Exiting child scene"))
    .Command("back", async ctx => await ctx.LeaveSceneAsync())
    .Command("ping", async ctx => await ctx.ReplyAsync("pong from child")));

#endregion

// Handle all other messages, this is not required!, just for example
bot.On(async ctx => { await ctx.ReplyAsync("not handled message"); });

Console.WriteLine("Launching bots...");

await botsManager.LaunchAsync();

Console.WriteLine("Bot is running... Press any key to exit.");
Console.ReadKey();