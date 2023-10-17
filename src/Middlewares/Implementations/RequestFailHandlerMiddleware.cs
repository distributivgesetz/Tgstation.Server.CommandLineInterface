namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using System.Net;
using CliFx.Exceptions;

public class RequestFailHandlerMiddleware : ICommandMiddleware
{
    private const string ServerCannotBeContacted =
        "Could not contact the TGS server. Make sure you are connected to the internet.";

    public async ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        try
        {
            await nextStep();
        }
        catch (WebException)
        {
            throw new CommandException(ServerCannotBeContacted);
        }
    }
}
