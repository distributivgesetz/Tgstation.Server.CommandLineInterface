namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using CliFx.Exceptions;
using Services;

public class EnsureCurrentRemoteMiddleware : ICommandMiddleware
{
    public const string RemoteUnsetErrorMessage =
        "No remote has been set, check \"tgs remote add --help\" for more details.";

    private readonly IRemoteRegistry remotes;

    public EnsureCurrentRemoteMiddleware(IRemoteRegistry remotes) => this.remotes = remotes;

    public ValueTask HandleCommandAsync(ICommandContext context, PipelineNext nextStep)
    {
        if (!this.remotes.HasCurrentRemote())
        {
            throw new CommandException(RemoteUnsetErrorMessage);
        }

        return nextStep();
    }
}
