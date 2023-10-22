namespace Tgstation.Server.CommandLineInterface.Extensions;

using System.Text;
using CliFx.Infrastructure;

public static class CliFxConsoleExtensions
{
    public static void WriteLine(this IConsole console, string message) => console.Output.WriteLine(message);

    public static Task WriteLineAsync(this IConsole console, string? message) =>
        console.Output.WriteLineAsync(message);

    public static Task WriteLineAsync(this IConsole console, ReadOnlyMemory<char> message, CancellationToken token = default) =>
        console.Output.WriteLineAsync(message, token);

    public static void Write(this IConsole console, string message) => console.Output.Write(message);

    public static void Write(this IConsole console, StringBuilder message) => console.Output.Write(message);

    public static Task WriteAsync(this IConsole console, string? message) =>
        console.Output.WriteAsync(message);

    public static Task WriteAsync(this IConsole console, StringBuilder message, CancellationToken token = default) =>
        console.Output.WriteAsync(message, token);

    public static Task WriteAsync(this IConsole console, ReadOnlyMemory<char> message, CancellationToken token = default) =>
        console.Output.WriteAsync(message, token);
}
