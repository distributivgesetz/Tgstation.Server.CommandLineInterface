namespace Tgstation.Server.CommandLineInterface.Models;

public abstract record ApiConverter<T>
{
    protected abstract T ToApi();
    public static implicit operator T(ApiConverter<T> inst) => inst.ToApi();
}
