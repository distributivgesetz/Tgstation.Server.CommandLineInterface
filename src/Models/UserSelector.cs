namespace Tgstation.Server.CommandLineInterface.Models;

using Api.Models;
using CliFx.Extensibility;

public sealed record UserSelector
{
    public UserSelector(long id) => this.Id = id;

    public UserSelector(string name) => this.Name = name;
    public long? Id { get; }
    public string? Name { get; }

    public static explicit operator UserName(UserSelector selector) => new()
    {
        Id = selector.Id ?? throw new InvalidCastException("Selector not translated")
    };
}

public sealed class UserSelectorConverter : BindingConverter<UserSelector>
{
    public override UserSelector Convert(string? rawValue) =>
        long.TryParse(rawValue, out var longValue) ?
            new UserSelector(longValue) :
            new UserSelector(rawValue ?? "");
}
