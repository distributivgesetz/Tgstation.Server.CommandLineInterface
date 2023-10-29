namespace Tgstation.Server.CommandLineInterface.Middlewares.Implementations;

using Client;
using CliFx.Exceptions;
using Services;

public class AutoLoginMiddleware : ICommandMiddleware
{
    private readonly IRemoteRegistry remotes;
    private readonly ISessionManager sessions;

    public AutoLoginMiddleware(ISessionManager sessions, IRemoteRegistry remotes)
    {
        this.sessions = sessions;
        this.remotes = remotes;
    }

    public async ValueTask HandleCommandAsync(ICommandContext context, PipelineNext nextStep)
    {
        var cancelToken = context.CancellationToken;
        var client = await this.sessions.TryResumeSession(cancelToken);

        if (client != null)
        {
            await nextStep();
            return;
        }

        this.sessions.DropSession(this.remotes.GetCurrentRemote().Name);

        try
        {
            await this.sessions.LoginToSession(context.Console, cancellationToken: cancelToken);
        }
        catch (UnauthorizedException e)
        {
            throw new CommandException("Invalid username or password.", innerException: e);
        }
        catch (BadLoginException e)
        {
            throw new CommandException(e.Message, innerException: e);
        }

        await nextStep();
    }
}
