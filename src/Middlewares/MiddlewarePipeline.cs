namespace Tgstation.Server.CommandLineInterface.Middlewares;

using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

public interface IMiddlewarePipelineBuilder
{
    void AddMiddleware<T>() where T : ICommandMiddleware;
    IMiddlewarePipeline Build();
}

public class MiddlewarePipelineBuilder : IMiddlewarePipelineBuilder
{
    private readonly IList<Type> registeredTypes = new List<Type>();

    private readonly IServiceProvider provider;

    public MiddlewarePipelineBuilder(IServiceProvider provider) => this.provider = provider;

    public void AddMiddleware<T>() where T : ICommandMiddleware =>
        this.registeredTypes.Add(typeof(T));

    public IMiddlewarePipeline Build() =>
        ActivatorUtilities.CreateInstance<MiddlewarePipeline>(this.provider, this.registeredTypes);
}

public interface IMiddlewarePipelineConfigurator
{
    void UseMiddleware<T>(params object[] arguments) where T : ICommandMiddleware;
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
    private readonly IList<Type> availableMiddlewares;
    private readonly IServiceProvider provider;
    private readonly List<ICommandMiddleware> middlewaresInUse = new();

    public MiddlewarePipeline(IList<Type> middlewares, IServiceProvider provider)
    {
        this.availableMiddlewares = middlewares;
        this.provider = provider;
    }

    public void UseMiddleware<T>(params object[] arguments) where T : ICommandMiddleware
    {
        if (!this.availableMiddlewares.Contains(typeof(T)))
        {
            throw new ArgumentException("Middleware not registered", nameof(T));
        }

        this.middlewaresInUse.Add(
            (ICommandMiddleware)ActivatorUtilities.CreateInstance(this.provider, typeof(T), arguments));
    }

    public ValueTask RunAsync(IConsole console, Func<IConsole, ValueTask> commandMain)
    {
        var context = new MiddlewareContext(console);

        var next = new PipelineNext(() => commandMain(context.Console));

        for (var index = this.middlewaresInUse.Count - 1; index >= 0; index--)
        {
            var middleware = this.middlewaresInUse[index];
            var current = next;
            next = () => middleware.HandleCommandAsync(context, current);
        }

        return next();
    }

    private sealed class MiddlewareContext : IMiddlewareContext
    {
        public MiddlewareContext(IConsole console) => this.Console = console;

        public IConsole Console { get; }
    }
}
