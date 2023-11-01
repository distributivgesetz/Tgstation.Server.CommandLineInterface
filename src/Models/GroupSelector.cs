namespace Tgstation.Server.CommandLineInterface.Models;

using CliFx.Extensibility;

public sealed record GroupSelector
{
    public GroupSelector(long id) => this.Id = id;

    public GroupSelector(string name) => this.Name = name;
    public long? Id { get; }
    public string? Name { get; }
}

public sealed class GroupSelectorConverter : BindingConverter<GroupSelector>
{
    public override GroupSelector Convert(string? rawValue) =>
        long.TryParse(rawValue, out var longValue) ?
            new GroupSelector(longValue) :
            new GroupSelector(rawValue ?? "");
}
