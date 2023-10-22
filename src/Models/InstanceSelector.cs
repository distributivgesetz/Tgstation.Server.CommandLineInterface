namespace Tgstation.Server.CommandLineInterface.Models;

using Api.Models;
using Api.Models.Response;
using CliFx.Extensibility;

public sealed record InstanceSelector
{
    public long? Id { get; }
    public string? Name { get; }

    public InstanceSelector(long id) => this.Id = id;

    public InstanceSelector(string name) => this.Name = name;

    public static explicit operator Instance(InstanceSelector selector) => new InstanceResponse
    {
        Id = selector.Id ?? throw new InvalidCastException("Selector not translated")
    };
}

public sealed class InstanceSelectorConverter : BindingConverter<InstanceSelector>
{
    public override InstanceSelector Convert(string? rawValue) =>
        long.TryParse(rawValue, out var longValue) ?
            new InstanceSelector(longValue) :
            new InstanceSelector(rawValue ?? "");
}
