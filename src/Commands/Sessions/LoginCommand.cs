namespace Tgstation.Server.CommandLineInterface.Commands.Sessions;

using CliFx.Attributes;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("login", Description = "Logs in with an account.")]
public sealed class LoginCommand : BaseCommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ISessionManager sessions;

    public LoginCommand(IRemoteRegistry remotes, ISessionManager sessions)
    {
        this.remotes = remotes;
        this.sessions = sessions;
    }

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares) =>
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var remote = this.remotes.GetCurrentRemote();

        if (this.sessions.HasSession(remote.Name))
        {
            await context.Console.Output.WriteLineAsync($"Using session for {remote.Name}.");
        }

        await this.sessions.LoginToSession(context.Console);
    }
}
