namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Server;

using CliFx.Attributes;
using Middlewares;
using Sessions;

[Command("instance server configure")]
public class ServerConfigureCommand : BaseSessionCommand
{
    protected override ValueTask RunCommandAsync(ICommandContext context) =>
        throw new NotImplementedException("This command is not yet implemented"); // TODO
}
