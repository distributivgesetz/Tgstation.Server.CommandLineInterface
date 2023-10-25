namespace Tgstation.Server.CommandLineInterface.Services;

using System.Diagnostics;
using System.Reflection;

public interface IApplicationInfo
{
    string Name { get; }
    string Version { get; }
    string VersionInfo { get; }
    string BasePath { get; }
}

public sealed class ApplicationInfo : IApplicationInfo
{
    static ApplicationInfo()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblyVersion = executingAssembly.GetName().Version!.ToString();
        AppVersion = assemblyVersion;
        AppVersionInfo = Environment.ProcessPath != null ?
            FileVersionInfo.GetVersionInfo(Environment.ProcessPath).ProductVersion ?? assemblyVersion :
            assemblyVersion;
    }

    public const string AppName = "tgs";
    public static readonly string AppVersion;
    public static readonly string AppVersionInfo;

    public string Name => AppName;
    public string Version => AppVersion;
    public string VersionInfo => AppVersionInfo;
    public string BasePath => AppContext.BaseDirectory;
}
