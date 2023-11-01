namespace Tgstation.Server.CommandLineInterface.Models;

using CliFx.Extensibility;

public sealed record InstanceSelector
{
    public InstanceSelector(long id) => this.Id = id;

    public InstanceSelector(string name) => this.Name = name;
    public long? Id { get; }
    public string? Name { get; }
}

public sealed class InstanceSelectorConverter : BindingConverter<InstanceSelector>
{
    public override InstanceSelector Convert(string? rawValue) =>
        long.TryParse(rawValue, out var longValue) ?
            new InstanceSelector(longValue) :
            new InstanceSelector(rawValue ?? "");
}
