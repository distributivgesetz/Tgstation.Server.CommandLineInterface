namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using CliFx.Exceptions;
using Services;

public class EnsureCurrentSessionMiddleware : ICommandMiddleware
{
    private readonly IRemoteRegistry remotes;

    private const string RemoteUnsetErrorMessage =
        "No remote has been registered, check \"tgs remote add --help\" for more details.";

    public EnsureCurrentSessionMiddleware(IRemoteRegistry remotes) => this.remotes = remotes;

    public ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        if (!this.remotes.HasCurrentRemote())
        {
            throw new CommandException(RemoteUnsetErrorMessage);
        }

        return nextStep();
    }
}
