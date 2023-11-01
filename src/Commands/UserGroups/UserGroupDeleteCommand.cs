namespace Tgstation.Server.CommandLineInterface.Commands.UserGroups;

using CliFx.Attributes;
using CliFx.Exceptions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("group create")]
public class UserGroupDeleteCommand : BaseSessionCommand
{
    private readonly IGroupManager groups;
    private readonly ISessionManager sessions;

    public UserGroupDeleteCommand(ISessionManager sessions, IGroupManager groups)
    {
        this.sessions = sessions;
        this.groups = groups;
    }

    [CommandParameter(0, Converter = typeof(GroupSelectorConverter))]
    public required GroupSelector Group { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.sessions.ResumeSession(context.CancellationToken);
        var groupId = await this.groups.SelectGroup(this.Group, context.CancellationToken);
        if (groupId == null)
        {
            throw new CommandException($"Could not find group {this.Group.Name}");
        }

        await client.Groups.Delete(groupId, context.CancellationToken);
    }
}
