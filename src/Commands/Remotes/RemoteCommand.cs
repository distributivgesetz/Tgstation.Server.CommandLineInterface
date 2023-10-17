namespace Tgstation.Server.CommandLineInterface.Commands.Remotes;

using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Services;

[Command("remote",
     Description = "Displays the currently used remote. If a name param is given, tries to set the current remote."),
 UsedImplicitly]
public class RemoteCommand : BaseCommand
{
    public const string RemoteUnsetErrorMessage =
        "No remote has been registered, check \"tgs remote add --help\" for more details.";

    private readonly IRemoteRegistry remotes;

    [CommandParameter(0, Description = "Sets the currently used remote.", IsRequired = false)]
    public string? Name { get; [UsedImplicitly] init; }

    public RemoteCommand(IRemoteRegistry registry) => this.remotes = registry;

    protected override ValueTask RunCommandAsync(IConsole console)
    {
        if (this.Name == null)
        {
            console.Output.WriteLine(this.remotes.HasCurrentRemote()
                ? this.remotes.GetCurrentRemote().Name
                : "No remote currently in use.");
        }
        else
        {
            if (this.remotes.HasCurrentRemote() && this.remotes.GetCurrentRemote().Name == this.Name)
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
}
