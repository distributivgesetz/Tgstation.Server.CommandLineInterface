namespace Tgstation.Server.CommandLineInterface.Commands;

using System.Globalization;
using System.Text;
using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Middlewares;
using Middlewares.Implementations;
using Services;

[Command("status"), UsedImplicitly]
public class StatusCommand : BaseCommand
{
    private readonly ITgsClientManager manager;
    private readonly IRemoteRegistry remotes;

    public StatusCommand(IRemoteRegistry remotes, ITgsClientManager manager)
    {
        this.manager = manager;
        this.remotes = remotes;
    }

    protected override void ConfigureMiddlewares(IMiddlewarePipelineConfigurator middlewares)
    {
        middlewares.UseMiddleware<EnsureCurrentSessionMiddleware>();
        middlewares.UseMiddleware<RequestFailHandlerMiddleware>();
    }

    protected override async ValueTask RunCommandAsync(IConsole console)
    {
        var currentRemote = this.remotes.GetCurrentRemote();

        await console.Output.WriteLineAsync("Fetching server status...");

        var res = await this.manager.PingServer(currentRemote.Host);

        var statusReadout = new StringBuilder();

        statusReadout.AppendLine(CultureInfo.InvariantCulture, $"\nStatus of TGS server at {currentRemote.Host}:\n");

        statusReadout.AppendLine(CultureInfo.InvariantCulture, $"Server version: {res.Version} - " +
            $"API version: {res.ApiVersion} - " +
            $"DMAPI version: {res.DMApiVersion}");

        statusReadout.AppendLine(CultureInfo.InvariantCulture, $"Server update in progress: {res.UpdateInProgress}");
        statusReadout.AppendLine(CultureInfo.InvariantCulture, $"Server OS: {(res.WindowsHost ? "Windows" : "Unix")}");

        statusReadout.AppendLine(CultureInfo.InvariantCulture, $"Instance limit: {res.InstanceLimit} " +
            $"{(res.ValidInstancePaths != null ?
                    $" Instance paths: {string.Join(';', res.ValidInstancePaths)}" :
                    ""
                )}");

        statusReadout.AppendLine(CultureInfo.InvariantCulture, $"Linked Swarm servers: {(res.SwarmServers != null ?
                "\n" + string.Join('\n', res.SwarmServers.Select(swarm =>
                    $"\tIdentifier: {swarm.Identifier} Address: {swarm.PublicAddress} " +
                    $"Is Swarm Controller: {swarm.Controller}")) :
                "None"
            )}");

        statusReadout.AppendLine(CultureInfo.InvariantCulture,
            $"User limit: {res.UserLimit} - User group limit: {res.UserGroupLimit} - " +
            $"Minimum password length: {res.MinimumPasswordLength}");

        await console.Output.WriteAsync(statusReadout);
    }
}
