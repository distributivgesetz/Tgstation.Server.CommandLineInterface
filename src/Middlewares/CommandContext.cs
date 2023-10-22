namespace Tgstation.Server.CommandLineInterface.Middlewares;

using CliFx.Infrastructure;

public interface ICommandContext
{
    IConsole Console { get; }
    CancellationToken CancellationToken => this.Console.RegisterCancellationHandler();
}

public sealed class CommandContext : ICommandContext
{
    public CommandContext(IConsole console) => this.Console = console;

    public IConsole Console { get; }
}
