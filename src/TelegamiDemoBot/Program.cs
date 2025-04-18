using Microsoft.Extensions.DependencyInjection;
using Telegami;
using Telegami.Extensions;
using Telegami.Sessions.LiteDB;
using TelegamiDemoBot.Middlewares;
using TelegamiDemoBot.OrderDemo;

var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("Please set the BOT_TOKEN environment variable!");
    return;
}

var serviceCollection = new ServiceCollection();

serviceCollection
    .AddTelegamiBot( config =>
    {
        config.Token = token;
    })
    .AddCommands(typeof(Program))
    .AddMiddlewares(typeof(Program))
    .AddLiteDBSessions();

var serviceProvider = serviceCollection.BuildServiceProvider();

var botsManager = serviceProvider.GetRequiredService<TelegamiBotsManager>();
var bot = botsManager.Get();
bot.Use<LoggerMiddleware>();

bot.Start(async ctx =>
{
    var msg = """
              Hello! I'm a Telegami Demo Bot.";
              /order_pizza - demo to show Scenes
              """;
    await ctx.SendAsync(msg);
});

bot.Command("order_pizza", async ctx => await ctx.EnterSceneAsync(PizzaOrderScene.SceneName));

bot.AddScene(new PizzaOrderScene());

Console.WriteLine("Launching bots...");
await botsManager.LaunchAsync();

Console.WriteLine("Bot is running... Press Control+C to exit");

var tcs = new TaskCompletionSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // Prevent the process from terminating immediately
    Console.WriteLine("Exiting app..");
    tcs.SetResult();
};

await tcs.Task;
