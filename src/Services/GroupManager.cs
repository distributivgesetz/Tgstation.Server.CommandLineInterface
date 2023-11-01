namespace Tgstation.Server.CommandLineInterface.Services;

using Api.Models;
using Api.Models.Internal;
using Models;

public interface IGroupManager
{
    ValueTask<UserGroup?> SelectGroup(GroupSelector selector, CancellationToken token = default);
    ValueTask<EntityId?> SelectGroupId(GroupSelector selector, CancellationToken token = default);
}

public class GroupManager : IGroupManager
{
    private readonly ISessionManager sessions;

    public GroupManager(ISessionManager sessions) => this.sessions = sessions;

    public async ValueTask<UserGroup?> SelectGroup(GroupSelector selector, CancellationToken token = default)
    {
        if (selector.Id != null)
        {
            return new UserGroup {Id = selector.Id};
        }

        var client = await this.sessions.ResumeSession(token);
        var users = await client.Groups.List(null, token);
        return users.FirstOrDefault(u => u.Name == selector.Name);
    }

    public async ValueTask<EntityId?> SelectGroupId(GroupSelector selector, CancellationToken token = default)
    {
        if (selector.Id != null)
        {
            return new EntityId {Id = selector.Id};
        }

        var client = await this.sessions.ResumeSession(token);
        var users = await client.Groups.List(null, token);
        return users.FirstOrDefault(u => u.Name == selector.Name);
    }
}
