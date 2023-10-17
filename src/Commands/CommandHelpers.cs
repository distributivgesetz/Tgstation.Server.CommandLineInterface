namespace Tgstation.Server.CommandLineInterface.Commands;

using CliFx;
using CliFx.Exceptions;

public static class CommandHelpers
{
    private const string ServerCannotBeContacted =
        "Could not contact this TGS server. Make sure you are connected to the internet.";

    public static async Task TryMakeRequest(this ICommand _, Func<Task> requestCode)
    {
        try
        {
            await requestCode();
        }
        catch (HttpRequestException)
        {
            throw new CommandException(ServerCannotBeContacted);
        }
    }

    public static async Task<T> TryMakeRequest<T>(this ICommand _, Func<Task<T>> requestCode)
    {
        try
        {
            return await requestCode();
        }
        catch (HttpRequestException)
        {
            throw new CommandException(ServerCannotBeContacted);
        }
    }
}
