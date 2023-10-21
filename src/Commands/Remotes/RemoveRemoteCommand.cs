namespace Tgstation.Server.CommandLineInterface.Commands.Remotes;

using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using Services;

[Command("remote remove", Description = "Unregisters a remote.")]
public sealed class RemoveRemoteCommand : ICommand
{
    private readonly IRemoteRegistry remotes;

    [CommandParameter(0, Description = "The remote to be unregistered.")]
    public required string Name { get; init; }

    public RemoveRemoteCommand(IRemoteRegistry remotes) => this.remotes = remotes;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!this.remotes.ContainsRemote(this.Name))
        {
            throw new CommandException($"Remote {this.Name} does not exist.");
        }

        this.remotes.RemoveRemote(this.Name);
        this.remotes.SaveRemotes();
        return default;
    }
}
