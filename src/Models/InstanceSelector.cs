namespace Tgstation.Server.CommandLineInterface.Models;

using Api.Models;
using Api.Models.Response;
using CliFx.Extensibility;

public sealed record InstanceSelector : ApiConverter<Instance>
{
    public long? Id { get; }
    public string? Name { get; }

    public InstanceSelector(long id) => this.Id = id;

    public InstanceSelector(string name) => this.Name = name;

    protected override Instance ToApi() =>
        this.Id != null ?
            new InstanceResponse { Id = this.Id } :
            throw new InvalidOperationException("Selector not translated");

    public static implicit operator long(InstanceSelector inst) =>
        inst.Id ?? throw new InvalidCastException("Selector not translated");
}

public sealed class InstanceSelectorConverter : BindingConverter<InstanceSelector>
{
    public override InstanceSelector Convert(string? rawValue) =>
        long.TryParse(rawValue, out var longValue) ?
            new InstanceSelector(longValue) :
            new InstanceSelector(rawValue ?? "");
}
