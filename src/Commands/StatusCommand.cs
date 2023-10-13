namespace Tgstation.Server.CommandLineInterface.Commands;

using System.Globalization;
using System.Text;
using Api.Models.Response;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global UnusedType.Global ClassNeverInstantiated.Global

[Command("status")]
public class StatusCommand : ICommand
{
    private readonly ITgsSessionManager manager;
    private readonly IRemoteRegistry remotes;

    public StatusCommand(IRemoteRegistry remotes, ITgsSessionManager manager)
    {
        this.manager = manager;
        this.remotes = remotes;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (this.remotes.CurrentRemote == null)
        {
            throw new CliFxException(RemoteCommand.RemoteUnsetErrorMessage);
        }

        ServerInformationResponse res;

        await console.Output.WriteLineAsync("Fetching server status...");

        try
        {
            res = await this.manager.PingServer(this.remotes.CurrentRemote.Value.Host);
        }
        catch (TgsBadResponseException e)
        {
            throw new CliFxException("Could not fetch server status.", innerException: e);
        }

        var currentRemote = this.remotes.CurrentRemote.Value;

        var statusReadout = new StringBuilder();

        statusReadout.AppendLine(CultureInfo.InvariantCulture, $"Status of TGS server at {currentRemote.Host}:\n");

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

        statusReadout.Append(CultureInfo.InvariantCulture,
            $"User limit: {res.UserLimit} User group limit: {res.UserGroupLimit} " +
            $"Minimum password length: {res.MinimumPasswordLength}");

        await console.Output.WriteLineAsync(statusReadout);
    }
}
