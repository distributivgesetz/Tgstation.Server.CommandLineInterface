namespace Tgstation.Server.CommandLineInterface.Models;

using System.Globalization;
using Api.Models;
using Api.Models.Response;
using CliFx.Extensibility;

public sealed record InstanceSelector(long Id) : ApiConverter<Instance>
{
    protected override Instance ToApi() => new InstanceResponse {Id = this.Id};
    public static implicit operator long(InstanceSelector inst) => inst.Id;
}

public class InstanceSelectorConverter : BindingConverter<InstanceSelector>
{
    public override InstanceSelector Convert(string? rawValue) =>
        !string.IsNullOrWhiteSpace(rawValue) ?
            new InstanceSelector(long.Parse(rawValue, CultureInfo.InvariantCulture)):
            new InstanceSelector(0);
}
