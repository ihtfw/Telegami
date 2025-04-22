Modern library for building Telegram bots.

It builds on top of the low-level [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) library, which handles communication with the Telegram Bot API.

# Basic Usage Examples

```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.AddTelegamiBot("BOT_TOKEN_FROM_BOT_FATHER");

var serviceProvider = serviceCollection.BuildServiceProvider();

var botsManager = serviceProvider.GetRequiredService<TelegamiBotsManager>();
var bot = botsManager.Get();

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

await botsManager.LaunchAsync();
```

That's it! Bot is working and can handle **/start** and **/custom** commands.

More information at [GitHub](https://github.com/ihtfw/Telegami)
