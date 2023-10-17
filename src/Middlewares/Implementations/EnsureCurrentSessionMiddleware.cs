namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using CliFx.Exceptions;
using Commands.RemoteManagement;
using Services;

public class EnsureCurrentSessionMiddleware : ICommandMiddleware
{
    private readonly IRemoteRegistry remotes;

    public EnsureCurrentSessionMiddleware(IRemoteRegistry remotes) => this.remotes = remotes;

    public ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep)
    {
        if (!this.remotes.HasCurrentRemote())
        {
            throw new CommandException(RemoteCommand.RemoteUnsetErrorMessage);
        }

        return nextStep();
    }
}
