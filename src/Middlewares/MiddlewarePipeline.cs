namespace Tgstation.Server.CommandLineInterface.Middlewares;

using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

public interface IMiddlewarePipelineConfigurator
{
    void UseMiddleware<T>() where T : ICommandMiddleware;
}

public interface IMiddlewarePipelineRunner
{
    ValueTask RunAsync(IConsole console, Func<ICommandContext, ValueTask> commandMain);
}

public interface IMiddlewarePipeline : IMiddlewarePipelineRunner, IMiddlewarePipelineConfigurator
{
}

public sealed class MiddlewarePipeline : IMiddlewarePipeline
{
    private readonly List<ICommandMiddleware> middlewaresInUse = new();
    private readonly IServiceProvider provider;

    public MiddlewarePipeline(IServiceProvider provider) => this.provider = provider;

    public void UseMiddleware<T>() where T : ICommandMiddleware
    {
        if (this.middlewaresInUse.Any(m => m is T))
        {
            throw new ArgumentException($"Middleware {typeof(T).FullName} exists in container already");
        }

        this.middlewaresInUse.Add((ICommandMiddleware)ActivatorUtilities.CreateInstance(this.provider, typeof(T)));
    }

    public ValueTask RunAsync(IConsole console, Func<ICommandContext, ValueTask> commandMain)
    {
        var context = new CommandContext(console);

        var next = new PipelineNext(() => commandMain(context));

        for (var index = this.middlewaresInUse.Count - 1; index >= 0; index--)
        {
            var middleware = this.middlewaresInUse[index];
            var current = next;
            next = () => middleware.HandleCommandAsync(context, current);
        }

        return next();
    }
}
