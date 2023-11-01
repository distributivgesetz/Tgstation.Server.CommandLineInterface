namespace Tgstation.Server.CommandLineInterface.Commands.UserGroups;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Services;
using Sessions;

[Command("group create")]
public class UserGroupCreateCommand : BaseSessionCommand
{
    private readonly ISessionManager sessions;

    public UserGroupCreateCommand(ISessionManager sessions) => this.sessions = sessions;

    [CommandParameter(0)] public required string GroupName { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.sessions.ResumeSession(context.CancellationToken);
        await client.Groups.Create(new UserGroupCreateRequest {Name = this.GroupName},
            context.CancellationToken);
    }
}
