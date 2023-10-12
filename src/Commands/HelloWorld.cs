using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Tgstation.Server.CommandLineInterface.Commands
{
    [Command("hello-world")]
    public class HelloWorld : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Hello, world!");
            return default;
        }
    }
}
