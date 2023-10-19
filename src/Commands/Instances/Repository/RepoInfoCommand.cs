namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using System.Text;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Services;

[Command("instance repo")]
public class RepoInfoCommand : BaseInstanceClientCommand
{
    public RepoInfoCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var instanceClient = await this.RequestInstanceClient(console);
        var info = await instanceClient.Repository.Read(console.RegisterCancellationHandler());

        var output = new StringBuilder();

        output.AppendLine(info.RemoteRepositoryName);
        output.AppendLine(info.RemoteRepositoryOwner);
        output.AppendLine(info.Origin!.ToString());
        output.AppendLine(info.Reference);

        await console.Output.WriteAsync(output);
    }
}
