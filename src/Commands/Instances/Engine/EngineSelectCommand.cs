namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Engine;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Exceptions;
using Middlewares;
using Models;
using Services;

[Command("instance engine select")]
public class EngineSelectCommand : BaseInstanceClientCommand
{
    [CommandParameter(0, Converter = typeof(InstanceSelectorConverter))]
    public required InstanceSelector Target { get; init; }

    [CommandOption("version", 'v')] public string? EngineVersion { get; init; }

    [CommandOption("file", 'z')] public string? FileName { get; init; }

    public EngineSelectCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        if (!((this.EngineVersion == null) ^ (this.FileName == null)))
        {
            throw new CommandException("Please specify either a file or an engine version string.");
        }

        if (this.FileName != null)
        {
            throw new NotImplementedException("Uploading custom versions is not supported yet");
        }

        if (Version.TryParse(this.EngineVersion!, out var parsedVersion))
        {
            var client = await this.RequestInstanceClient(this.Target, context.CancellationToken);
            await client.Byond.SetActiveVersion(new ByondVersionRequest
            {
                Version = parsedVersion
            }, null, context.CancellationToken);
        }
        else
        {
            throw new CommandException($"Engine version {this.EngineVersion} is invalid");
        }
    }
}
