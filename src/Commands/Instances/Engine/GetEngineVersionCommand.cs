namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Engine;

using System.Globalization;
using System.Text;
using CliFx.Attributes;
using Extensions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance engine")]
public class GetEngineVersionCommand : BaseSessionCommand
{
    private readonly IInstanceManager instances;

    public GetEngineVersionCommand(IInstanceManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        var currentVersion = await client.Byond.ActiveVersion(context.CancellationToken);
        var availableVersions = await client.Byond.InstalledVersions(null, context.CancellationToken);

        var output = new StringBuilder();
        output.AppendLine(CultureInfo.InvariantCulture, $"Current version: {currentVersion.Version!}");
        output.AppendLine("Available versions:");
        foreach (var version in availableVersions)
        {
            output.AppendLine(version.Version!.ToString());
        }

        await context.Console.WriteAsync(output.ToString());
    }
}
