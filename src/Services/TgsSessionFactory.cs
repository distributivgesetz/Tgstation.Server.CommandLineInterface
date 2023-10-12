using Tgstation.Server.Client;

namespace Tgstation.Server.CommandLineInterface.Services;

public interface ITgsSessionFactory
{
}

public class TgsSessionFactory : ITgsSessionFactory
{
    readonly IServerClientFactory serverClientFactory;

    public TgsSessionFactory(IServerClientFactory serverClientFactory)
    {
        this.serverClientFactory = serverClientFactory;
    }
    
    public IServerClient TryResumeSession()
    {
        serverClientFactory.CreateFromToken();
    }
}