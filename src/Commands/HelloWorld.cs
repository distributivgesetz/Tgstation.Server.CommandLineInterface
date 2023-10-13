// ReSharper disable UnusedAutoPropertyAccessor.Global UnusedType.Global ClassNeverInstantiated.Global

namespace Tgstation.Server.CommandLineInterface.Commands;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

[Command("echo")]
public class HelloWorld : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Hello, world!");
        return default;
    }
}
