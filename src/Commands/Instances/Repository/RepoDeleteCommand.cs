namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using CliFx.Attributes;
using CliFx.Infrastructure;
using Models;
using Services;

[Command("instance repo delete")]
public class RepoDeleteCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    public RepoDeleteCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var instanceClient = await this.RequestInstanceClient(this.Instance, console);
        await instanceClient.Repository.Delete(console.RegisterCancellationHandler());
    }
}
