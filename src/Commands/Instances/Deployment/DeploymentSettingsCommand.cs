namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Deployment;

using System.Globalization;
using System.Text;
using CliFx.Attributes;
using Extensions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance deploy")]
public class GetDeploymentSettingsCommand : BaseSessionCommand
{
    private readonly IInstanceClientManager instances;

    public GetDeploymentSettingsCommand(IInstanceClientManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        var res = await client.DreamMaker.Read(context.CancellationToken);

        var output = new StringBuilder();

        output.AppendLine(CultureInfo.InvariantCulture, $"Project name: {res.ProjectName}");
        output.AppendLine(CultureInfo.InvariantCulture, $"DMAPI validation settings:");
        output.AppendLine(CultureInfo.InvariantCulture, $"  API validation required: {res.RequireDMApiValidation}");
        output.AppendLine(CultureInfo.InvariantCulture, $"  API validation port: {res.ApiValidationPort}");
        output.AppendLine(CultureInfo.InvariantCulture,
            $"  API validation security level: {res.ApiValidationSecurityLevel.ToString()}");
        output.AppendLine(CultureInfo.InvariantCulture, $"  API validation timeout: {res.Timeout:c}");


        await context.Console.WriteAsync(output.ToString());
    }
}
