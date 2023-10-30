﻿namespace Tgstation.Server.CommandLineInterface.Commands.Instances.Engine;

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
        var response = await client.Byond.ActiveVersion(context.CancellationToken);
        await context.Console.WriteLineAsync(response.Version!.ToString());
    }
}
