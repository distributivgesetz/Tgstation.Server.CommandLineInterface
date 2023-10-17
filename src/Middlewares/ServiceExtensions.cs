namespace Tgstation.Server.CommandLineInterface.Services;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection UseMiddlewares(this IServiceCollection container,
        Func<IMiddlewarePipelineBuilder, IMiddlewarePipeline> configure)
    {
        container.AddSingleton<IMiddlewarePipelineConfigurator>(services =>
        {
            var builder = ActivatorUtilities.CreateInstance<MiddlewarePipelineBuilder>(services);
            return configure(builder);
        });
        return container;
    }
}
