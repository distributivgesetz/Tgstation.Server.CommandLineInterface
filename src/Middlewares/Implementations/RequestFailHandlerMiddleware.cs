namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using System.Net.Sockets;
using Client;
using CliFx.Exceptions;
using Services;

public class RequestFailHandlerMiddleware : ICommandMiddleware
{
    private readonly IRemoteRegistry remotes;

    public RequestFailHandlerMiddleware(IRemoteRegistry remotes) => this.remotes = remotes;

    public async ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        var host = this.remotes.GetCurrentRemote().Host;

        try
        {
            await nextStep();
        }
        catch (ApiException e)
        {
            throw new CommandException($"{host} returned an error! Code is {e.ErrorCode.ToString()}");
        }
        catch (HttpRequestException e) when (e.InnerException is SocketException)
        {
            throw new CommandException($"Request to {host} failed! Reason is {e.InnerException.Message}");
        }
    }
}
