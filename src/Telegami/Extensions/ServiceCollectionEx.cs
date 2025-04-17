using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Telegami.Commands;

namespace Telegami.Extensions;

public static class ServiceCollectionEx
{
    public static IServiceCollection AddTelegamiCommands(this IServiceCollection serviceCollection, params Type[] assemblyMarkers)
    {
        var assemblies = assemblyMarkers.Select(x => x.Assembly).Distinct().ToArray();
        return serviceCollection.AddTelegamiCommands(assemblies);
    }

    public static IServiceCollection AddTelegamiCommands(this IServiceCollection serviceCollection, params Assembly[] assemblies)
    {
        var commandTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && typeof(ITelegamiCommandHandler).IsAssignableFrom(x));

        foreach (var commandType in commandTypes)
        {
            serviceCollection.AddScoped(commandType);
        }

        return serviceCollection;
    }

    public static IServiceCollection AddTelegamiBot(this IServiceCollection serviceCollection, string token)
    {
        return serviceCollection.AddTelegamiBot(TelegamiBot.DefaultKey, config =>
        {
            config.Token = token;
        });
    }

    public static IServiceCollection AddTelegamiBot(this IServiceCollection serviceCollection, string key, string token)
    {
        return serviceCollection.AddTelegamiBot(key, config =>
        {
            config.Token = token;
        });
    }

    public static IServiceCollection AddTelegamiBot(this IServiceCollection serviceCollection, Action<TelegamiBotConfig> configure)
    {
        return AddTelegamiBot(serviceCollection, TelegamiBot.DefaultKey, configure);
    }

    public static IServiceCollection AddTelegamiBot(this IServiceCollection serviceCollection, string key, Action<TelegamiBotConfig> configure)
    {
        if (serviceCollection.All(x => x.ServiceType != typeof(TelegamiBotsManager)))
        {
            serviceCollection.AddSingleton<TelegamiBotsManager>();
        }

        serviceCollection.AddSingleton(x =>
        {
            var config = new TelegamiBotConfig();
            configure(config);

            return new TelegamiBot(x, key, config);
        });

        return serviceCollection;
    }

}