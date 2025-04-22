[![NuGet](https://img.shields.io/nuget/v/Telegami.svg)](https://www.nuget.org/packages/Telegami/)
[![Publish to NuGet](https://github.com/ihtfw/Telegami/actions/workflows/publish.yml/badge.svg)](https://github.com/ihtfw/Telegami/actions/workflows/publish.yml)

Telegami is the modern library for building Telegram bots. 
The goal of Telegami is to keep things straightforward and easy to use.

**Current status:** Active development — breaking changes may occur between releases.

# See in action

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

Check out [Wiki](https://github.com/ihtfw/Telegami/wiki) for more.
