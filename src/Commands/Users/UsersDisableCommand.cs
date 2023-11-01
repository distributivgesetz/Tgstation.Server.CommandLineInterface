namespace Tgstation.Server.CommandLineInterface.Commands.Users;

using Api.Models.Request;
using CliFx.Attributes;
using CliFx.Exceptions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("users disable")]
public class UsersDisableCommand : BaseSessionCommand
{
    private readonly ISessionManager sessions;
    private readonly IUserManager users;

    public UsersDisableCommand(ISessionManager sessions, IUserManager users)
    {
        this.sessions = sessions;
        this.users = users;
    }

    [CommandParameter(0, Converter = typeof(UserSelectorConverter))]
    public required UserSelector User { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.sessions.ResumeSession(context.CancellationToken);
        var userId = await this.users.SelectUserId(this.User);
        if (userId == null)
        {
            throw new CommandException($"Could not find {this.User.Name}");
        }

        await client.Users.Update(new UserUpdateRequest {Id = userId.Id, Enabled = false},
            context.CancellationToken);
    }
}
