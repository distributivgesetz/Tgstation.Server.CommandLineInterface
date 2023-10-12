using System;
using System.Collections.Generic;
using CliFx;
using Microsoft.Extensions.DependencyInjection;

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

    foreach (var command in commands) 
        services.AddTransient(command);

    return services.BuildServiceProvider();
}