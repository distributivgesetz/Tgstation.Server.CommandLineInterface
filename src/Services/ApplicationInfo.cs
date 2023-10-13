namespace Tgstation.Server.CommandLineInterface.Services;

using System.Reflection;

public interface IApplicationInfo
{
    string Name { get; }
    Version Version { get; }
}

public class ApplicationInfo : IApplicationInfo
{
    public const string ApplicationName = "TGS-CLI";

    public string Name => ApplicationName;
    public Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version!;
}
