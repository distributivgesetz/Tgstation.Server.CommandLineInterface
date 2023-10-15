namespace Tgstation.Server.CommandLineInterface.Services;

using Api.Models.Response;
using Client;

public interface ITgsClientManager
{
    Task<ServerInformationResponse> PingServer(Uri host);

    Task<IServerClient> CreateSessionWithToken(Uri host, string token, DateTimeOffset expiry,
        CancellationToken cancellationToken = default);

    Task<IServerClient> CreateSessionWithLogin(Uri host, string userName, string password,
        CancellationToken cancellationToken = default);
}

public class TgsClientManager : ITgsClientManager
{
    private readonly IServerClientFactory serverClientFactory;

    public TgsClientManager(IServerClientFactory serverClientFactory) => this.serverClientFactory = serverClientFactory;

    public async Task<IServerClient> CreateSessionWithToken(Uri host, string token, DateTimeOffset expiry,
        CancellationToken cancellationToken = default)
    {
        var client = this.serverClientFactory.CreateFromToken(host,
            new TokenResponse {Bearer = token, ExpiresAt = expiry});

        // try interacting with the api to see if the token works, just in case
        await client.ServerInformation(cancellationToken);

        return client;
    }

    public async Task<IServerClient> CreateSessionWithLogin(Uri host, string userName, string password,
        CancellationToken cancellationToken = default) =>
        await this.serverClientFactory.CreateFromLogin(host, userName, password, cancellationToken: cancellationToken);

    public async Task<ServerInformationResponse> PingServer(Uri host) =>
        await this.serverClientFactory.GetServerInformation(host);
}
