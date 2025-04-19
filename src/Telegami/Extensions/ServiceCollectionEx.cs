using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Telegami.Commands;
using Telegami.Middlewares;
using Telegami.Scenes;

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
    /// Registers IScene
    /// </summary>
    /// <returns></returns>
    public TelegamiBotBuilder AddScene<TScene>() where TScene : class, IScene
    {
        ServiceCollection.AddScoped<TScene>();
        return this;
    }

    /// <summary>
    /// Registers all IScene from assemblies
    /// </summary>
    /// <param name="assemblyMarkers"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddScenes(params Type[] assemblyMarkers)
    {
        var assemblies = assemblyMarkers.Select(x => x.Assembly).Distinct().ToArray();
        return AddScenes(assemblies);
    }

    /// <summary>
    /// Registers all IScene from assemblies
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddScenes(params Assembly[] assemblies)
    {
        var registeredTypes = ServiceCollection
            .Where(x => typeof(IScene).IsAssignableFrom(x.ServiceType))
            .Select(x => x.ServiceType)
            .ToHashSet();

        var foundTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && typeof(IScene).IsAssignableFrom(x));

        foreach (var type in foundTypes)
        {
            if (registeredTypes.Add(type))
            {
                ServiceCollection.AddScoped(type);
            }
        }

        return this;
    }


    /// <summary>
    /// Registers ITelegamiCommandHandler
    /// </summary>
    /// <returns></returns>
    public TelegamiBotBuilder AddCommand<TTelegamiCommandHandler>() where TTelegamiCommandHandler : class, ITelegamiCommandHandler
    {
        ServiceCollection.AddScoped<TTelegamiCommandHandler>();
        return this;
    }

    /// <summary>
    /// Registers all ITelegamiCommandHandler from assemblies
    /// </summary>
    /// <param name="assemblyMarkers"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddCommands(params Type[] assemblyMarkers)
    {
        var assemblies = assemblyMarkers.Select(x => x.Assembly).Distinct().ToArray();
        return AddCommands(assemblies);
    }

    /// <summary>
    /// Registers all ITelegamiCommandHandler from assemblies
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddCommands(params Assembly[] assemblies)
    {
        var commands = ServiceCollection
            .Where(x => typeof(ITelegamiCommandHandler).IsAssignableFrom(x.ServiceType))
            .Select(x => x.ServiceType)
            .ToHashSet();

        var commandTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && typeof(ITelegamiCommandHandler).IsAssignableFrom(x));

        foreach (var commandType in commandTypes)
        {
            if (commands.Add(commandType))
            {
                ServiceCollection.AddScoped(commandType);
            }
        }

        return this;
    }

    /// <summary>
    /// Registers ITelegamiMiddleware
    /// </summary>
    /// <returns></returns>
    public TelegamiBotBuilder AddMiddleware<TMiddleware>() where TMiddleware : class, ITelegamiMiddleware
    {
        ServiceCollection.AddScoped<TMiddleware>();
        return this;
    }

    /// <summary>
    /// Registers all ITelegamiMiddleware from assemblies
    /// </summary>
    /// <param name="assemblyMarkers"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddMiddlewares(params Type[] assemblyMarkers)
    {
        var assemblies = assemblyMarkers.Select(x => x.Assembly).Distinct().ToArray();
        return AddMiddlewares(assemblies);
    }

    /// <summary>
    /// Registers all ITelegamiMiddleware from assemblies
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public TelegamiBotBuilder AddMiddlewares(params Assembly[] assemblies)
    {
        var middlewares = ServiceCollection
            .Where(x => typeof(ITelegamiMiddleware).IsAssignableFrom(x.ServiceType))
            .Select(x => x.ServiceType)
            .ToHashSet();

        var middlewareTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && typeof(ITelegamiMiddleware).IsAssignableFrom(x));

        foreach (var middlewareType in middlewareTypes)
        {
            if (middlewares.Add(middlewareType))
            {
                ServiceCollection.AddScoped(middlewareType);
            }
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

    public static TelegamiBotBuilder AddTelegamiBot(this IServiceCollection serviceCollection, Action<IServiceProvider, TelegamiBotConfig> configure)
    {
        return AddTelegamiBot(serviceCollection, TelegamiBot.DefaultKey, configure);
    }

    public static TelegamiBotBuilder AddTelegamiBot(this IServiceCollection serviceCollection, Action<TelegamiBotConfig> configure)
    {
        return AddTelegamiBot(serviceCollection, TelegamiBot.DefaultKey, configure);
    }

    public static TelegamiBotBuilder AddTelegamiBot(this IServiceCollection serviceCollection, string key,
        Action<IServiceProvider, TelegamiBotConfig> configure)
    {
        if (serviceCollection.All(x => x.ServiceType != typeof(TelegamiBotsManager)))
        {
            serviceCollection.AddSingleton<TelegamiBotsManager>();
        }

        serviceCollection.AddSingleton(x =>
        {
            var config = new TelegamiBotConfig();
            configure(x, config);

            return new TelegamiBot(x, key, config);
        });

        return new TelegamiBotBuilder(key, serviceCollection);
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