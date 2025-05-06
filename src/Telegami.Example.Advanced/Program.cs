using Microsoft.Extensions.DependencyInjection;
using Telegami;
using Telegami.Example.Advanced.ImageCarousel;
using Telegami.Example.Advanced.Middlewares;
using Telegami.Example.Advanced.OrderPizza;
using Telegami.Example.Advanced.OrderPizza.BtnImpl;
using Telegami.Example.Advanced.OrderPizza.TextImpl;
using Telegami.Extensions;
using Telegami.Scenes;
using Telegram.Bot.Types.Enums;
using Utils = Telegami.Example.Advanced.Utils;

var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("Please set the BOT_TOKEN environment variable!");
    return;
}

var serviceCollection = new ServiceCollection();

serviceCollection
    .AddTelegamiBot(config =>
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

var commands = """
               /help - show help
               /date - show current date
               /order_pizza_text - demo to show Scenes with text and commands
               /order_pizza_btn - demo to show Scenes with text and commands and buttons
               /instance_scene - example how to create scene with separate class
               /image_carousel - interactive control to show images of cats
               """;
var bot = botsManager.Get();
bot.Use<LoggerMiddleware>()
    .Use<GlobalErrorHandlerMiddleware>()
    .Start(async ctx =>
    {
        var msg = $"Hello! I'm a Telegami Demo Bot.\n{commands}";
        await ctx.SendAsync(msg);
    })
    .Help(async ctx =>
    {
        var msg = $"Here what I can:\n{commands}";
        await ctx.SendAsync(msg);
    })
    .Command("order_pizza_text", async ctx => await ctx.EnterSceneAsync<TextPizzaOrderScene>())
    .Command("order_pizza_btn", async ctx => await ctx.EnterSceneAsync<BtnPizzaOrderScene>())
    .Command("date", async ctx => await ctx.SendAsync(DateTime.Now.ToString("O")))
    .Command("instance_scene", async ctx => await ctx.EnterSceneAsync("instance_scene"))
    .Command("image_carousel", async ctx => await ctx.EnterSceneAsync<ImageCarouselScene>())
    .AddScene<TextPizzaOrderScene>()
    .AddScene<BtnPizzaOrderScene>()
    .AddScene(new ImageCarouselScene()
    {
        Images = Utils.Assets.Cats()
    })
    .AddScene("instance_scene", new Scene()
        .Enter(async ctx => { await ctx.SendAsync("Send me a sticker! (or /back)"); })
        .Command("back", async ctx => { await ctx.LeaveSceneAsync(); })
        .On(MessageType.Sticker, async ctx => { await ctx.ReplyAsync("wow! nice stiker!"); })
        .On(async ctx => { await ctx.SendAsync("Send me a sticker! (or /back)"); })
    )
    .On(async ctx => { await ctx.SendAsync($"I understand only this commands:\n{commands}"); });

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