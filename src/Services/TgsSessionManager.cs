namespace Tgstation.Server.CommandLineInterface.Services;

using Api.Models;
using Api.Models.Response;
using Client;

public interface ITgsSessionManager
{
    Task<ServerInformationResponse> PingServer(Uri host);
}

public class TgsBadResponseException : Exception
{
    public TgsBadResponseException(string? message) : base(message)
    {
    }

    public TgsBadResponseException(string? message, Exception inner) : base(message, inner)
    {
    }
}

public class TgsSessionManager : ITgsSessionManager
{
    private readonly IServerClientFactory serverClientFactory;

    public TgsSessionManager(IServerClientFactory serverClientFactory) => this.serverClientFactory = serverClientFactory;

    public async Task<ServerInformationResponse> PingServer(Uri host)
    {
        ServerInformationResponse res;
        try
        {
            res = await this.serverClientFactory.GetServerInformation(host);
        }
        catch (HttpRequestException e)
        {
            throw new TgsBadResponseException(
                "Could not contact a TGS server at this URL. Please check for typos in the URL and " +
                "make sure you are connected to the internet.", e);
        }
        catch (ApiException e)
        {
            throw new TgsBadResponseException(e.ErrorCode != null
                ? $"API returned an error ({e.ErrorCode})." +
                  $"\nDescription: {e.ErrorCode.Value.Describe()} {e.AdditionalServerData}"
                : $"API returned an unknown error ({e.Message}).", e);
        }

        return res;
    }
}
