namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Deployment;

using Api.Models;
using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance deploy configure")]
public class DeploymentConfigureCommand : BaseSessionCommand
{
    private readonly IInstanceManager instances;

    public DeploymentConfigureCommand(IInstanceManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Instance { get; init; }

    [CommandOption("project", 'n')] public string? ProjectName { get; init; }

    [CommandOption("port", 'p')] public ushort? ValidationPort { get; init; }

    [CommandOption("level", 's', Converter = typeof(DreamDaemonSecurityConverter))]
    public DreamDaemonSecurity? SecurityLevel { get; init; }

    [CommandOption("validate", 'v')] public bool? ValidationRequired { get; init; }

    [CommandOption("timeout", 't')] public TimeSpan? Timeout { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.instances.RequestInstanceClient(this.Instance, context.CancellationToken);
        await client.DreamMaker.Update(
            new DreamMakerRequest
            {
                ProjectName = this.ProjectName,
                ApiValidationPort = this.ValidationPort,
                ApiValidationSecurityLevel = this.SecurityLevel,
                RequireDMApiValidation = this.ValidationRequired,
                Timeout = this.Timeout
            }, context.CancellationToken);
    }
}
