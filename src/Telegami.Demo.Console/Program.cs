// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;using Telegami;

Console.WriteLine("Hello, World!");

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

bot.Start(async (IMessageContext ctx) =>
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

bot.Hears("hello", async (IMessageContext ctx, MyCustomService myCustomService) =>
{
    await ctx.ReplyAsync("World!");
});

await bot.LaunchAsync();

Console.WriteLine("Bot is running... Press any key to exit.");
Console.ReadKey();

class MyCustomService
{
}
