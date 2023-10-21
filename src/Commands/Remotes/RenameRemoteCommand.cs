namespace Tgstation.Server.CommandLineInterface.Commands.Remotes;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

[Command("remote rename", Description = "Renames a remote.")]
public sealed class RenameRemoteCommand : ICommand
{
    [CommandParameter(0, Description = "The remote to be renamed.")]
    public required string Name { get; init; }

    [CommandParameter(1, Description = "The new name for the remote.")]
    public required string NewName { get; init; }

    public ValueTask ExecuteAsync(IConsole console) => throw new NotImplementedException();
}
