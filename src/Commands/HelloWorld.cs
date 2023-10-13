using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Tgstation.Server.CommandLineInterface.Commands;

[Command("echo")]
public class HelloWorld : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Hello, world!");
        return default;
    }
}