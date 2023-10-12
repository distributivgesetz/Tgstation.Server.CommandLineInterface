using CliFx;
using Microsoft.Extensions.DependencyInjection;
using Tgstation.Server.Client;
using Tgstation.Server.CommandLineInterface.Services;

return await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .UseTypeActivator(ConfigureServices)
    .SetExecutableName("tgs-cli") // :3c
    .SetDescription("Command line interface for tgstation-server")
    .Build()
    .RunAsync();

IServiceProvider ConfigureServices(IEnumerable<Type> commands)
{
    var services = new ServiceCollection();

    // Register services here

    services.AddSingleton<IServerClientFactory, ServerClientFactory>();
    services.AddSingleton<IPreferencesManager, PreferencesManager>();
    
    // Register commands
    
    foreach (var command in commands) 
        services.AddTransient(command);

    return services.BuildServiceProvider();
}