namespace Tgstation.Server.CommandLineInterface.Services;

using System.Reflection;

public interface IApplicationInfo
{
    string Name { get; }
    string Version { get; }
}

public sealed class ApplicationInfo : IApplicationInfo
{
    public const string ApplicationName = "TGS-CLI";
    public static readonly string ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

    public string Name => ApplicationName;
    public string Version => ApplicationVersion;
}
