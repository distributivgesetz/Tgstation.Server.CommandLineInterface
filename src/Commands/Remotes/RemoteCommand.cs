namespace Tgstation.Server.CommandLineInterface.Commands.Remotes;

using CliFx.Attributes;
using CliFx.Exceptions;
using Extensions;
using Middlewares;
using Services;

[Command("remote",
    Description = "Displays the currently used remote. If a name param is given, tries to set the current remote.")]
public sealed class RemoteCommand : BaseCommand
{
    private readonly IRemoteRegistry remotes;

    public RemoteCommand(IRemoteRegistry registry) => this.remotes = registry;

    [CommandParameter(0, Description = "The name of the remote to set.", IsRequired = false)]
    public string? Name { get; init; }

    [CommandOption("unset", 'u', Description = "Unsets the current remote.")]
    public bool Unset { get; init; }

    protected override ValueTask RunCommandAsync(ICommandContext context)
    {
        if (this.Unset)
        {
            this.remotes.SetCurrentRemote(null);
            this.remotes.SaveRemotes();
            return default;
        }

        if (this.Name == null)
        {
            context.Console.WriteLine(this.remotes.HasCurrentRemote() ?
                this.remotes.GetCurrentRemote().Name :
                "No remote currently set.");
        }
        else
        {
            if (this.remotes.HasCurrentRemote() && this.remotes.GetCurrentRemote().Name == this.Name)
            {
                return default;
            }

            if (!this.remotes.ContainsRemote(this.Name))
            {
                throw new CommandException("This remote has not been registered before.");
            }

            this.remotes.SetCurrentRemote(this.Name);
            this.remotes.SaveRemotes();
        }

        return default;
    }
}
