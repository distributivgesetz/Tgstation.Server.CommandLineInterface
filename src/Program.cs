using System.Net.Http.Headers;
using CliFx;
using Microsoft.Extensions.DependencyInjection;
using Tgstation.Server.Client;
using Tgstation.Server.CommandLineInterface.Services;

return await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .UseTypeActivator(ConfigureServices)
    .SetExecutableName(ApplicationInfo.ApplicationName.ToLower(System.Globalization.CultureInfo.CurrentCulture))
    .SetDescription("Command line interface for tgstation-server")
    .Build()
    .RunAsync();

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

    services.AddSingleton<IPreferencesManager, PreferencesManager>();
    services.AddSingleton<IRemoteRegistry, RemoteRegistry>();
    services.AddSingleton<ITgsSessionManager, TgsSessionManager>();

    // Register commands

    foreach (var command in commands)
    {
        services.AddTransient(command);
    }

    return services.BuildServiceProvider();
}
