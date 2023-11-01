namespace Tgstation.Server.CommandLineInterface.Commands.Users;

using Api.Models.Request;
using CliFx.Attributes;
using Middlewares;
using Services;
using Sessions;

[Command("users create")]
public class UsersCreateCommand : BaseSessionCommand
{
    private readonly ISessionManager sessions;

    public UsersCreateCommand(ISessionManager sessions) => this.sessions = sessions;

    [CommandParameter(0)] public required string Username { get; init; }

    [CommandParameter(1)] public required string Password { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.sessions.ResumeSession(context.CancellationToken);
        await client.Users.Create(new UserCreateRequest {Name = this.Username, Password = this.Password},
            context.CancellationToken);
    }
}
