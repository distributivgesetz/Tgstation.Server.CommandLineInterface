namespace Tgstation.Server.CommandLineInterface.Middlewares;

using Commands;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection UseCommand(this IServiceCollection container, Type commandType) =>
        container.AddTransient(commandType, services =>
        {
            var inst = ActivatorUtilities.CreateInstance(services, commandType);

            if (!commandType.IsSubclassOf(typeof(BaseCommand)))
            {
                return inst;
            }

            var baseCommand = (BaseCommand)inst;
            baseCommand.UseMiddlewares(services.GetRequiredService<IMiddlewarePipeline>());

            return inst;
        });
}
