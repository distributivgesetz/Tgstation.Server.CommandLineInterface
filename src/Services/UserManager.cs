namespace Tgstation.Server.CommandLineInterface.Services;

using Api.Models;
using Models;

public interface IUserManager
{
    ValueTask<UserName?> SelectUser(UserSelector selector, CancellationToken token = default);
    ValueTask<EntityId?> SelectUserId(UserSelector selector, CancellationToken token = default);
}

public class UserManager : IUserManager
{
    private readonly ISessionManager sessions;

    public UserManager(ISessionManager sessions) => this.sessions = sessions;

    public async ValueTask<UserName?> SelectUser(UserSelector selector, CancellationToken token = default)
    {
        if (selector.Id != null)
        {
            return new UserName {Id = selector.Id};
        }

        var client = await this.sessions.ResumeSession(token);
        var users = await client.Users.List(null, token);
        return users.FirstOrDefault(u => u.Name == selector.Name);
    }

    public async ValueTask<EntityId?> SelectUserId(UserSelector selector, CancellationToken token = default)
    {
        if (selector.Id != null)
        {
            return new EntityId {Id = selector.Id};
        }

        var client = await this.sessions.ResumeSession(token);
        var users = await client.Users.List(null, token);
        return users.FirstOrDefault(u => u.Name == selector.Name);
    }
}
