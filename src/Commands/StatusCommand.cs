using System.Text;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using Tgstation.Server.Api.Models.Response;
using Tgstation.Server.CommandLineInterface.Services;

namespace Tgstation.Server.CommandLineInterface.Commands;

[Command("status")]
public class StatusCommand : ICommand
{
    private readonly ITgsSessionManager _manager;
    private readonly IRemoteRegistry _remotes;
    private readonly IPreferencesManager _prefs;

    public StatusCommand(IRemoteRegistry remotes, IPreferencesManager prefs, ITgsSessionManager manager)
    {
        _manager = manager;
        _remotes = remotes;
        _prefs = prefs;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (_remotes.CurrentRemote == null)
        {
            throw new CliFxException(RemoteCommand.RemoteUnsetErrorMessage);
        }

        ServerInformationResponse res;
        
        await console.Output.WriteLineAsync("Fetching server status...");

        try
        {
            res = await _manager.PingServer(_remotes.CurrentRemote.Value.Host);
        }
        catch (TgsBadResponseException e)
        {
            throw new CliFxException("Could not fetch server status.", innerException: e);
        }

        var currentRemote = _remotes.CurrentRemote.Value;

        var statusReadout = new StringBuilder();
        
        statusReadout.AppendLine($"Status of TGS server at {currentRemote.Host}:\n");
        
        statusReadout.AppendLine($"Server version: {res.Version} - " +
                                 $"API version: {res.ApiVersion} - " +
                                 $"DMAPI version: {res.DMApiVersion}");
        statusReadout.AppendLine($"Server update in progress: {res.UpdateInProgress}");
        statusReadout.AppendLine($"Server OS: {(res.WindowsHost ? "Windows" : "Unix")}");
        
        statusReadout.AppendLine($"Instance limit: {res.InstanceLimit}" +
            $"{(res.ValidInstancePaths != null ? 
                $" Instance paths: {string.Join(';', res.ValidInstancePaths)}" : 
                ""
                )}");
        
        statusReadout.AppendLine($"Linked Swarm servers: {(res.SwarmServers != null ? 
            "\n" + string.Join('\n', res.SwarmServers.Select(swarm => 
                $"\tIdentifier: {swarm.Identifier} Address: {swarm.PublicAddress} " +
                $"Is Swarm Controller: {swarm.Controller}")) : 
            "None"
            )}");
        statusReadout.Append($"User limit: {res.UserLimit} User group limit: {res.UserGroupLimit} " +
            $"Minimum password length: {res.MinimumPasswordLength}");

        await console.Output.WriteLineAsync(statusReadout);
    }
}