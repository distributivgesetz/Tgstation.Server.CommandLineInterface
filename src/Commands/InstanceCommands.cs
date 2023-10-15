namespace Tgstation.Server.CommandLineInterface.Commands;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Services;

[Command("instances list")]
public class ListInstancesCommand : ICommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ISessionManager sessions;

    public ListInstancesCommand(IRemoteRegistry remotes, ISessionManager sessions)
    {
        this.remotes = remotes;
        this.sessions = sessions;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        this.FailIfNoRemote(this.remotes);
        await this.TryMakeRequest(async () =>
        {
            throw new NotImplementedException();
        });
    }
}
