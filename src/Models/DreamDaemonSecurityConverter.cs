namespace Tgstation.Server.CommandLineInterface.Models;

using Api.Models;
using CliFx.Extensibility;

public class DreamDaemonSecurityConverter : BindingConverter<DreamDaemonSecurity>
{
    public override DreamDaemonSecurity Convert(string? rawValue) =>
        Enum.TryParse<DreamDaemonSecurity>(rawValue, out var res) ? res : throw new FormatException();
}
