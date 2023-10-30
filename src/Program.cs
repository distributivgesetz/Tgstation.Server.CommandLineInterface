// autogenerated program class is not auto-sealed :pain:

#pragma warning disable CA1852

using System.Net.Http.Headers;
using CliFx;
using Microsoft.Extensions.DependencyInjection;
using Tgstation.Server.Client;
using Tgstation.Server.CommandLineInterface.Extensions;
using Tgstation.Server.CommandLineInterface.Middlewares;
using Tgstation.Server.CommandLineInterface.Models;
using Tgstation.Server.CommandLineInterface.Services;
#if DEBUG
using System.Diagnostics;
#endif

#if DEBUG
var sw = Stopwatch.StartNew();
#endif

var app = new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .UseTypeActivator(ConfigureServices)
    .SetExecutableName(ApplicationInfo.AppName.ToLowerInvariant())
    .SetVersion(ApplicationInfo.AppVersionInfo)
    .SetDescription("Command line interface for tgstation-server")
    .Build();

#if DEBUG
sw.Stop();
Console.WriteLine($"CLI framework build took {sw.ElapsedMilliseconds}ms");
#endif

return await app.RunAsync();

static IServiceProvider ConfigureServices(IEnumerable<Type> commands)
{
    var services = new ServiceCollection();

    // Register services here

    services.AddSingleton<IApplicationInfo, ApplicationInfo>();

    services.AddSingleton<IServerClientFactory, ServerClientFactory>(serviceProvider =>
    {
        var appInfo = serviceProvider.GetRequiredService<IApplicationInfo>();
        return new ServerClientFactory(new ProductHeaderValue(appInfo.Name, appInfo.Version));
    });

    services.AddSingleton<IPersistenceManager, PersistenceManager>();
    services.AddSingleton<IRemoteRegistry, RemoteRegistry>();
    services.AddSingleton<ISessionManager, SessionManager>();
    services.AddSingleton<ITgsClientManager, TgsClientManager>();
    services.AddSingleton<IInstanceManager, InstanceManager>();
    services.AddSingleton<IMiddlewarePipeline, MiddlewarePipeline>();

    // Register converters here

    services.AddSingleton<InstanceSelectorConverter>();

    // Handle commands

    foreach (var command in commands)
    {
        services.UseCommand(command);
    }

    return services.BuildServiceProvider();
}
