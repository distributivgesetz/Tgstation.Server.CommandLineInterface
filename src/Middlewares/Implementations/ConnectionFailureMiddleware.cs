namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using System.Net;
using CliFx.Exceptions;

public class ConnectionFailureMiddleware : ICommandMiddleware
{
    public async ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        try
        {
            await nextStep();
        }
        catch (WebException)
        {
            throw new CommandException("Internet issue");
        }
    }
}
