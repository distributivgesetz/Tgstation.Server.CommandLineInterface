namespace Tgstation.Server.CommandLineInterface.Services;

using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Middlewares;

public interface IMiddlewarePipelineBuilder
{
    void AddMiddleware<T>() where T : ICommandMiddleware;
    IMiddlewarePipeline Build();
}

public class MiddlewarePipelineBuilder : IMiddlewarePipelineBuilder
{
    private readonly IDictionary<Type, ICommandMiddleware> middlewareTypesToInstances =
        new Dictionary<Type, ICommandMiddleware>();

    private readonly IServiceProvider provider;

    public MiddlewarePipelineBuilder(IServiceProvider provider) => this.provider = provider;

    public void AddMiddleware<T>() where T : ICommandMiddleware =>
        this.middlewareTypesToInstances.Add(typeof(T), ActivatorUtilities.CreateInstance<T>(this.provider));

    public IMiddlewarePipeline Build() =>
        ActivatorUtilities.CreateInstance<MiddlewarePipeline>(this.provider, this.middlewareTypesToInstances);
}

public interface IMiddlewarePipelineConfigurator
{
    void UseMiddleware<T>() where T : ICommandMiddleware;
}

public interface IMiddlewarePipelineRunner
{
    ValueTask RunAsync(IConsole console, Func<IConsole, ValueTask> commandMain);
}

public interface IMiddlewarePipeline : IMiddlewarePipelineRunner, IMiddlewarePipelineConfigurator
{
}

public interface IMiddlewareContext
{
    IConsole Console { get; }
}

public class MiddlewarePipeline : IMiddlewarePipeline
{
    private readonly IDictionary<Type, ICommandMiddleware> availableMiddlewares;
    private readonly List<ICommandMiddleware> registeredMiddlewares = new();

    public MiddlewarePipeline(IDictionary<Type, ICommandMiddleware> middlewares)
        => this.availableMiddlewares = middlewares;

    public void UseMiddleware<T>() where T : ICommandMiddleware
        => this.registeredMiddlewares.Add(this.availableMiddlewares[typeof(T)]);

    public ValueTask RunAsync(IConsole console, Func<IConsole, ValueTask> commandMain)
    {
        var context = new MiddlewareContext(console);

        var next = new PipelineNext(() => commandMain(context.Console));

        for (var index = this.registeredMiddlewares.Count - 1; index >= 0; index--)
        {
            var middleware = this.registeredMiddlewares[index];
            var pipeline = next;
            next = () => middleware.HandleCommandAsync(context, pipeline);
        }

        return next();
    }

    private class MiddlewareContext : IMiddlewareContext
    {
        public MiddlewareContext(IConsole console) => this.Console = console;

        public IConsole Console { get; }
    }
}
