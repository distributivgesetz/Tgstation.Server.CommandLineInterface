namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Repository;

using System.Text;
using CliFx.Attributes;
using Extensions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance repo", Description = "Displays information about an instance's repository and its settings.")]
public sealed class RepoInfoCommand : BaseSessionCommand
{
    private readonly IInstanceManager instances;

    public RepoInfoCommand(IInstanceManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter), Description = "The instance target.")]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var instanceClient = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        var info = await instanceClient.Repository.Read(context.CancellationToken);

        var output = new StringBuilder();

        // TODO: stop being lazy and reformat this
        output.AppendLine(info.RemoteRepositoryName);
        output.AppendLine(info.RemoteRepositoryOwner);
        output.AppendLine(info.Origin!.ToString());
        output.AppendLine(info.Reference);

        await context.Console.WriteAsync(output);
    }
}
