using System.Diagnostics;
using System.Net.Http.Headers;
using CliFx;
using Microsoft.Extensions.DependencyInjection;
using Tgstation.Server.Client;
using Tgstation.Server.CommandLineInterface.Middlewares;
using Tgstation.Server.CommandLineInterface.Middlewares.Implementations;
using Tgstation.Server.CommandLineInterface.Services;

var sw = Stopwatch.StartNew();

var app = new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .UseTypeActivator(ConfigureServices)
    .SetExecutableName(ApplicationInfo.ApplicationName.ToLowerInvariant())
    .SetDescription("Command line interface for tgstation-server")
    .Build();

sw.Stop();

Console.WriteLine($"Startup took {sw.ElapsedMilliseconds}ms");

return await app.RunAsync();

static IServiceProvider ConfigureServices(IEnumerable<Type> commands)
{
    var services = new ServiceCollection();

    // Register services here

    services.AddSingleton<IApplicationInfo, ApplicationInfo>();

    services.AddSingleton<IServerClientFactory, ServerClientFactory>(serviceProvider =>
    {
        var appInfo = serviceProvider.GetRequiredService<IApplicationInfo>();
        return new ServerClientFactory(new ProductHeaderValue(appInfo.Name, appInfo.Version.ToString()));
    });

    services.AddSingleton<IPersistenceManager, PersistenceManager>();
    services.AddSingleton<IRemoteRegistry, RemoteRegistry>();
    services.AddSingleton<ISessionManager, SessionManager>();
    services.AddSingleton<ITgsClientManager, TgsClientManager>();

    // Handle middlewares

    services.UseMiddlewares(builder =>
    {
        builder.AddMiddleware<ConnectionFailureMiddleware>();
        builder.AddMiddleware<EnsureCurrentSessionMiddleware>();
        return builder.Build();
    });

    // Handle commands

    foreach (var command in commands)
    {
        services.UseCommand(command);
    }

    return services.BuildServiceProvider();
}

// Fixes CA1852 in this file
internal sealed partial class Program
{
}
