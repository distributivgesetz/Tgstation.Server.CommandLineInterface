namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using System.Net.Sockets;
using Client;
using CliFx.Exceptions;

public class RequestFailHandlerMiddleware : ICommandMiddleware
{
    public async ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        try
        {
            await nextStep();
        }
        catch (ApiException e)
        {
            throw new CommandException($"API returned an error: {e.ErrorCode.ToString()}");
        }
        catch (HttpRequestException e) when (e.InnerException?
                                                 .GetType().IsSubclassOf(typeof(SocketException))
                                             ?? false)
        {
            throw new CommandException($"Request to API failed: {e.InnerException.Message}");
        }
    }
}
