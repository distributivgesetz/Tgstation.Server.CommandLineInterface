namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using Client;
using CliFx.Exceptions;
using Services;

public class UserUnauthorizedHandler : ICommandMiddleware
{
    public async ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        try
        {
            await nextStep();
        }
        catch (UnauthorizedException e)
        {
            throw new CommandException("Invalid server credentials.", innerException: e);
        }
        catch (BadLoginException e)
        {
            throw new CommandException(e.Message, innerException: e);
        }
    }
}
