
using Microsoft.Extensions.DependencyInjection;
using Telegami;
using Telegami.Demo.Console.Middlewares;
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

var serviceProvider = serviceCollection.BuildServiceProvider();

var bot = new TelegamiBot(token)
{
    ServiceProvider = serviceProvider
};

bot.Use<LoggerMiddleware>();
bot.Use<GlobalErrorHandlerMiddleware>();

bot.Start(async (IMessageContext ctx, MyCustomService service) =>
{
    await ctx.ReplyAsync("Hello! I'm a bot. How can I help you?");
});

bot.Help(async (IMessageContext ctx) =>
{
    await ctx.ReplyAsync("This is a help message.");
});

bot.Settings(async (IMessageContext ctx) =>
{
    await ctx.ReplyAsync("This is a settings message.");
});

bot.Command("custom", async (IMessageContext ctx) =>
{
    await ctx.ReplyAsync($"this is custom command handler. args was: '{ctx.BotCommand!.Arguments}'");
});

bot.Command("echo", async ctx =>
{
    await ctx.EnterSceneAsync("echo");
});

bot.Command("person", async ctx =>
{
    await ctx.EnterSceneAsync("person");
});

bot.Command("error", () => throw new Exception("This is a test exception"));

bot.On(MessageType.Sticker, async (IMessageContext ctx) =>
{
    await ctx.ReplyAsync($"What a nice sticker!");
});

bot.Hears("hello", async (IMessageContext ctx, MyCustomService myCustomService) =>
{
    await ctx.ReplyAsync("World!");
});

bot.Hears("world", async ctx =>
{
    await ctx.ReplyAsync("hello!");
});

bot.On(async ctx =>
{
    await ctx.ReplyAsync("not handled message");
});

var echoScene = new Scene("echo");
echoScene.Enter(async ctx => await ctx.ReplyAsync("echo scene"));
echoScene.Leave(async ctx => await ctx.ReplyAsync("exiting echo scene"));
echoScene.Command("back", async ctx => await ctx.LeaveSceneAsync());
echoScene.Command("leave", async ctx => await ctx.LeaveSceneAsync());
echoScene.On(MessageType.Text, async ctx => await ctx.ReplyAsync(ctx.Message.Text ?? ""));
echoScene.On(async ctx => await ctx.ReplyAsync("Only text messages please"));
bot.AddScene(echoScene);

var personCardScene = new Scene("person");
personCardScene.Enter(async ctx => await ctx.SendAsync("Hi! What's your name?"));
personCardScene.Leave(async ctx =>
{
    var name = ctx.Session!.Get("name");
    var lastName = ctx.Session!.Get("lastName");
    var age = ctx.Session!.Get("age");

    await ctx.ReplyAsync($"Your name is {name} {lastName}, you are {age} years old.");
});

personCardScene.On(MessageType.Text, async ctx =>
{
    var name = ctx.Session!.Get("name");
    if (string.IsNullOrEmpty(name))
    {
        ctx.Session!.Set("name", ctx.Message!.Text!);
        await ctx.ReplyAsync($"What's your last name?");
        return;
    }

    var lastName = ctx.Session!.Get("lastName");
    if (string.IsNullOrEmpty(lastName))
    {
        ctx.Session!.Set("lastName", ctx.Message!.Text!);
        await ctx.ReplyAsync($"What's your age?");
        return;
    }

    ctx.Session!.Set("age", ctx.Message!.Text!);
    await ctx.LeaveSceneAsync();
});
bot.AddScene(personCardScene);

await bot.LaunchAsync();

Console.WriteLine("Bot is running... Press any key to exit.");
Console.ReadKey();

class MyCustomService
{
}
