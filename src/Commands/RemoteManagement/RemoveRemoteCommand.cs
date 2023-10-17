namespace Tgstation.Server.CommandLineInterface.Commands.RemoteManagement;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

[Command("remote remove", Description = "Unregisters a remote.")]
public class RemoveRemoteCommand : ICommand
{
    [CommandParameter(0)] public required string Name { get; init; }

    public ValueTask ExecuteAsync(IConsole console) => throw new NotImplementedException();
}
