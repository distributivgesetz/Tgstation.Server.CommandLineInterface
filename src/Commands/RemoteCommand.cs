// ReSharper disable UnusedAutoPropertyAccessor.Global UnusedType.Global ClassNeverInstantiated.Global

namespace Tgstation.Server.CommandLineInterface.Commands;

using Client;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using Services;

[Command("remote",
    Description = "Displays the currently used remote. If a name param is given, tries to set the current remote.")]
public class RemoteCommand : BaseCommand
{
    public const string RemoteUnsetErrorMessage =
        "No remote has been registered, check \"tgs remote add --help\" for more details.";

    private readonly IRemoteRegistry remotes;

    [CommandParameter(0, Description = "Sets the currently used remote.", IsRequired = false)]
    public string? Name { get; init; }

    public RemoteCommand(IRemoteRegistry registry) => this.remotes = registry;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (this.Name == null)
        {
            console.Output.WriteLine(this.remotes.CurrentRemote != null
                ? this.remotes.CurrentRemote.Value.Name
                : "No remote currently in use.");
        }
        else
        {
            if (this.remotes.CurrentRemote?.Name == this.Name)
            {
                return default;
            }

            if (!this.remotes.ContainsRemote(this.Name))
            {
                throw new CliFxException("This remote has not been registered before.");
            }

            this.remotes.SetCurrentRemote(this.Name);
            this.remotes.SaveRemotes();
        }

        return default;
    }

    protected override ValueTask RunCommandAsync(IConsole console) => default;
}

[Command("remote list", Description = "List available remotes.")]
public class RemoteListCommand : ICommand
{
    private readonly IRemoteRegistry remotes;

    public RemoteListCommand(IRemoteRegistry remotes) => this.remotes = remotes;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (this.remotes.AvailableRemotes.Count == 0)
        {
            console.Output.WriteLine("None available.");
            return default;
        }

        foreach (var remoteKey in this.remotes.AvailableRemotes)
        {
            console.Output.WriteLine(remoteKey);
        }
        return default;
    }
}

[Command("remote add", Description = "Adds a remote.")]
public class RemoteAddCommand : ICommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ITgsClientManager tgsManager;

    [CommandParameter(0, Description = "The name to use for this remote.")]
    public required string Name { get; init; }

    [CommandParameter(1, Description = "The URL of the server's API.")]
    public required string Url { get; init; }

    public RemoteAddCommand(IRemoteRegistry remotes, ITgsClientManager tgsManager)
    {
        this.remotes = remotes;
        this.tgsManager = tgsManager;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        // check if the user gave us garbage

        if (this.remotes.ContainsRemote(this.Name))
        {
            throw new CliFxException(
                "Cannot register remote because another one with the same name already exists. " +
                "(Did you mean to use \"tgs remote rename\"?)");
        }

        if (!Uri.TryCreate(this.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) || uri.Query != "")
        {
            throw new CliFxException(
                "Cannot use given URL because it is not valid. " +
                "Please use a complete HTTP/S URL, without a query string.");
        }

        // check if the server is available

        await console.Output.WriteLineAsync("Checking for server availability...");

        try
        {
            await this.TryMakeRequest(() => this.tgsManager.PingServer(uri));
        }
        catch (ApiException e)
        {
            throw new CliFxException(
                $"Cannot add this remote because the API could not contact the given server.\n{e.Message}");
        }

        // we're good

        this.remotes.AddRemote(this.Name, new Uri(this.Url));
        this.remotes.SaveRemotes();

        await console.Output.WriteLineAsync("New remote registered successfully.");
    }
}

[Command("remote rename", Description = "Renames a remote.")]
public class RenameRemoteCommand : ICommand
{
    [CommandParameter(0)] public required string Name { get; init; }
    [CommandParameter(1)] public required string NewName { get; init; }

    public ValueTask ExecuteAsync(IConsole console) => throw new NotImplementedException();
}

[Command("remote remove", Description = "Unregisters a remote.")]
public class RemoveRemoteCommand : ICommand
{
    [CommandParameter(0)] public required string Name { get; init; }

    public ValueTask ExecuteAsync(IConsole console) => throw new NotImplementedException();
}
