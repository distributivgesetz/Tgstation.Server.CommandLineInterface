namespace Tgstation.Server.CommandLineInterface.Middlewares;

using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
    private readonly IServiceProvider provider;
    private readonly List<ICommandMiddleware> middlewaresInUse = new();

    public MiddlewarePipeline(IServiceProvider provider) => this.provider = provider;

    public void UseMiddleware<T>() where T : ICommandMiddleware =>
        this.middlewaresInUse.Add((ICommandMiddleware)ActivatorUtilities.CreateInstance(this.provider, typeof(T)));

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
