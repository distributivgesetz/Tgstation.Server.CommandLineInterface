using Tgstation.Server.CommandLineInterface.Services;

namespace Tgstation.Server.CommandLineInterface.Preferences;

[Preferences("remotes")]
public record RemotesPreferences(TgsRemote[] Remotes);

public record TgsRemote(Uri Host);