namespace Tgstation.Server.CommandLineInterface.Commands;

using CliFx;
using CliFx.Infrastructure;
using Services;

public abstract class BaseCommand : ICommand
{
    private IMiddlewarePipelineRunner? pipeline;

    public ValueTask ExecuteAsync(IConsole console) => this.pipeline!.RunAsync(console, this.RunCommandAsync);

    public void UseMiddlewares(IMiddlewarePipeline insertedPipeline)
    {
        this.ConfigureMiddlewares(insertedPipeline);
        this.pipeline = insertedPipeline;
    }

    protected virtual void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
    }

    protected abstract ValueTask RunCommandAsync(IConsole console);
}
