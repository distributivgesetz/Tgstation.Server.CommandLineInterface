namespace Tgstation.Server.CommandLineInterface.Commands.Users;

using System.Globalization;
using System.Text;
using CliFx.Attributes;
using Extensions;
using Middlewares;
using Services;
using Sessions;

[Command("users list")]
public class UsersListCommand : BaseSessionCommand
{
    private readonly ISessionManager sessions;

    public UsersListCommand(ISessionManager sessions) => this.sessions = sessions;

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var client = await this.sessions.ResumeSession(context.CancellationToken);
        var users = await client.Users.List(null, context.CancellationToken);

        var output = new StringBuilder();

        foreach (var user in users)
        {
            output.AppendLine(CultureInfo.InvariantCulture, $"{user.Name} ({user.Id})");
        }

        await context.Console.WriteAsync(output.ToString());
    }
}
