namespace Tgstation.Server.CommandLineInterface.Commands;

using System.Net.Sockets;
using System.Security.Authentication;
using Api.Models;
using Client;
using CliFx;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using Middlewares;

public abstract class BaseCommand : ICommand
{
    private IMiddlewarePipelineRunner pipeline = null!;

    public ValueTask ExecuteAsync(IConsole console) => this.pipeline.RunAsync(console, this.RunCommandAsync);

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

public static class RequestHelpers
{
    public static async ValueTask TryServerRequestAsync(Func<ValueTask> requestAction, Uri? host)
    {
        var requestVerb = host != null ? $"Request to {host}" : "Request";

        try
        {
            await requestAction();
        }
        catch (ApiException e) when (e.ErrorCode != null)
        {
            throw new CommandException($"{requestVerb} failed with an API error! ({e.ErrorCode})\n" +
                                       $"{e.ErrorCode.Value.Describe()}" +
                                       (e.AdditionalServerData != null ? $"\n{e.AdditionalServerData}" : ""));
        }
        catch (HttpRequestException e) when (e.InnerException is SocketException or AuthenticationException)
        {
            throw new CommandException($"{requestVerb} failed! Reason is:\n{e.InnerException.Message}");
        }
    }
}
