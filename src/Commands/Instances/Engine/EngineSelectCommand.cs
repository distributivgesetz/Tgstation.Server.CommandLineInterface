namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Engine;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Exceptions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("instance engine select")]
public class EngineSelectCommand : BaseSessionCommand
{
    private readonly IInstanceClientManager instances;

    public EngineSelectCommand(IInstanceClientManager instances) => this.instances = instances;

    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Target { get; init; }

    [CommandOption("version", 'v', Converter = typeof(VersionConverter))]
    public Version? EngineVersion { get; init; }

    [CommandOption("file", 'z')] public string? FileName { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        if (!((this.EngineVersion == null) ^ (this.FileName == null)))
        {
            throw new CommandException("Please specify either a file or an engine version.");
        }

        if (this.FileName != null)
        {
            throw new NotImplementedException("Uploading custom versions is not supported yet");
        }

        var client = await this.instances.RequestInstanceClient(this.Target, context.CancellationToken);
        await client.Byond.SetActiveVersion(new ByondVersionRequest {Version = this.EngineVersion}, null,
            context.CancellationToken);
    }
}
