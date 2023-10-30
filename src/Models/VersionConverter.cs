namespace Tgstation.Server.CommandLineInterface.Models;

using CliFx.Extensibility;

public class VersionConverter : BindingConverter<Version>
{
    public override Version Convert(string? rawValue) =>
        Version.TryParse(rawValue, out var res) ? res : throw new FormatException();
}
