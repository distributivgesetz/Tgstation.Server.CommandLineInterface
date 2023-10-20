namespace Tgstation.Server.CommandLineInterface.Commands.Remotes;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Services;

[Command("remote list", Description = "List available remotes.")]
public sealed class RemoteListCommand : ICommand
{
    private readonly IRemoteRegistry remotes;

    public RemoteListCommand(IRemoteRegistry remotes) => this.remotes = remotes;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (this.remotes.AvailableRemotes.Count == 0)
        {
            console.Output.WriteLine("None available.");
            return default;
        }

        foreach (var remoteKey in this.remotes.AvailableRemotes)
        {
            console.Output.WriteLine(remoteKey);
        }

        return default;
    }
}
