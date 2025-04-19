using Microsoft.Extensions.DependencyInjection;
using Telegami;
using Telegami.Extensions;
using Telegami.Sessions.LiteDB;
using TelegamiDemoBot.Middlewares;
using TelegamiDemoBot.OrderPizza;
using TelegamiDemoBot.OrderPizza.BtnImpl;
using TelegamiDemoBot.OrderPizza.TextImpl;

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
        config.EnableGlobalDebugDumpCommand = true;
    })
    .AddScenes(typeof(Program))
    .AddCommands(typeof(Program))
    .AddMiddlewares(typeof(Program));
    //.AddLiteDBSessions();

serviceCollection.AddSingleton<PizzaMenu>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var botsManager = serviceProvider.GetRequiredService<TelegamiBotsManager>();
var bot = botsManager.Get();
bot.Use<LoggerMiddleware>();

var commands = """
               /help - show help
               /date - show current date
               /order_pizza_text - demo to show Scenes with text and commands
               /order_pizza_btn - demo to show Scenes with text and commands and buttons
               """;

bot.Start(async ctx =>
{
    var msg = $"Hello! I'm a Telegami Demo Bot.\n{commands}";
    await ctx.SendAsync(msg);
});
bot.Help(async ctx =>
{
    var msg = $"Here what I can:\n{commands}";
    await ctx.SendAsync(msg);
});

bot.Command("order_pizza_text", async ctx => await ctx.EnterSceneAsync(TextPizzaOrderScene.SceneName));
bot.Command("order_pizza_btn", async ctx => await ctx.EnterSceneAsync(BtnPizzaOrderScene.SceneName));
bot.Command("date", async ctx => await ctx.SendAsync(DateTime.Now.ToString("O")));

bot.AddScene<TextPizzaOrderScene>(TextPizzaOrderScene.SceneName);
bot.AddScene<BtnPizzaOrderScene>(BtnPizzaOrderScene.SceneName);

bot.On(async ctx =>
{
    await ctx.SendAsync($"I understand only this commands:\n{commands}");
});

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
