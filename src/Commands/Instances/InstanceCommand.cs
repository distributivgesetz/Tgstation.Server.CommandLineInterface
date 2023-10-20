namespace Tgstation.Server.CommandLineInterface.Commands.Instances;

using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;

[Command("instance")]
public class InstanceCommand : ICommand
{
    public ValueTask ExecuteAsync(IConsole console) =>
        throw new CommandException("Please provide an option.", showHelp: true);
}
