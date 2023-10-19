namespace Tgstation.Server.CommandLineInterface.Commands.Remotes;

using Client;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Services;

[Command("remote add", Description = "Adds a remote."), UsedImplicitly]
public sealed class RemoteAddCommand : ICommand
{
    private readonly IRemoteRegistry remotes;
    private readonly ITgsClientManager tgsManager;

    [CommandParameter(0, Description = "The name to use for this remote.")]
    public required string Name { get; [UsedImplicitly] init; }

    [CommandParameter(1, Description = "The URL of the server's API.")]
    public required string Url { get; [UsedImplicitly] init; }

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
            throw new CommandException(
                "Cannot register remote because another one with the same name already exists. " +
                "(Did you mean to use \"tgs remote rename\"?)");
        }

        if (!Uri.TryCreate(this.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) || uri.Query != "")
        {
            throw new CommandException(
                "Cannot use given URL because it is not valid. " +
                "Please use a complete HTTP/S URL, without a query string.");
        }

        // check if the server is available

        await console.Output.WriteLineAsync("Checking for server availability...");

        try
        {
            await this.tgsManager.PingServer(uri);
        }
        catch (ApiException e)
        {
            throw new CommandException(
                $"Cannot add this remote because the API could not contact the given server.\n{e.Message}");
        }

        // we're good

        this.remotes.AddRemote(this.Name, new Uri(this.Url));
        this.remotes.SaveRemotes();

        await console.Output.WriteLineAsync("New remote registered successfully.");
    }
}
