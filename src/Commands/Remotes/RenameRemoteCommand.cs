namespace Tgstation.Server.CommandLineInterface.Commands.Remotes;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

[Command("remote rename", Description = "Renames a remote.")]
public class RenameRemoteCommand : ICommand
{
    [CommandParameter(0)] public required string Name { get; init; }
    [CommandParameter(1)] public required string NewName { get; init; }

    public ValueTask ExecuteAsync(IConsole console) => throw new NotImplementedException();
}
