using CliFx;
using Microsoft.Extensions.DependencyInjection;

return await new CliApplicationBuilder()
    .SetExecutableName("tgs-cli") // :3c
    .UseTypeActivator(ConfigureServices)
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync();

IServiceProvider ConfigureServices(IEnumerable<Type> commands)
{
    var services = new ServiceCollection();

    foreach (var command in commands) 
        services.AddTransient(command);

    return services.BuildServiceProvider();
}