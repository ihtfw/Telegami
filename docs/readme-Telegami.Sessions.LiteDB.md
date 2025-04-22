# Usage example

By default, Telegami uses in-memory storage. While this is acceptable, it is not persistent and also increases the application's memory footprint.
To address this, we can use [LiteDB](https://github.com/litedb-org/LiteDB) for persistent storage.

```CSharp
var serviceCollection = new ServiceCollection();

serviceCollection
    .AddTelegamiBot("YOUR_BOT_TOKEN")
    .AddLiteDBSessions(); // <-- add this line to use LiteDB for session storage

var serviceProvider = serviceCollection.BuildServiceProvider();

var botsManager = serviceProvider.GetRequiredService<TelegamiBotsManager>();
var bot = botsManager.Get();

// add handlers to your bot

await botsManager.LaunchAsync();
```

More information at [GitHub](https://github.com/ihtfw/Telegami)
