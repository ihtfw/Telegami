using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Telegami.Commands;

namespace Telegami.Extensions;

public class TelegamiBotBuilder
{
    public string Key { get; }
    public IServiceCollection ServiceCollection { get; }

    public TelegamiBotBuilder(string key, IServiceCollection serviceCollection)
    {
        Key = key;
        ServiceCollection = serviceCollection;
    }

    /// <summary>
    /// Should be invoked only once per assembly!
    /// </summary>
    /// <param name="assemblyMarkers"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddCommands(params Type[] assemblyMarkers)
    {
        var assemblies = assemblyMarkers.Select(x => x.Assembly).Distinct().ToArray();
        return AddCommands(assemblies);
    }

    /// <summary>
    /// Should be invoked only once per assembly!
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddCommands(params Assembly[] assemblies)
    {
        var commandTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && typeof(ITelegamiCommandHandler).IsAssignableFrom(x));

        foreach (var commandType in commandTypes)
        {
            ServiceCollection.AddScoped(commandType);
        }

        return this;
    }
}

public static class ServiceCollectionEx
{
    public static TelegamiBotBuilder AddTelegamiBot(this IServiceCollection serviceCollection, string token)
    {
        return serviceCollection.AddTelegamiBot(TelegamiBot.DefaultKey, config =>
        {
            config.Token = token;
        });
    }

    public static TelegamiBotBuilder AddTelegamiBot(this IServiceCollection serviceCollection, string key, string token)
    {
        return serviceCollection.AddTelegamiBot(key, config =>
        {
            config.Token = token;
        });
    }

    public static TelegamiBotBuilder AddTelegamiBot(this IServiceCollection serviceCollection, Action<TelegamiBotConfig> configure)
    {
        return AddTelegamiBot(serviceCollection, TelegamiBot.DefaultKey, configure);
    }

    public static TelegamiBotBuilder AddTelegamiBot(this IServiceCollection serviceCollection, string key, Action<TelegamiBotConfig> configure)
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

        return new TelegamiBotBuilder(key, serviceCollection);
    }

}