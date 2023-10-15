namespace Tgstation.Server.CommandLineInterface.Commands;

using CliFx;
using CliFx.Exceptions;
using Preferences;
using Services;

public static class CommandHelpers
{
    private const string ServerCannotBeContacted =
        "Could not contact this TGS server. Make sure you are connected to the internet.";

    public static void FailIfNoRemote(this ICommand _, IRemoteRegistry remotes)
    {
        if (remotes.CurrentRemote.HasValue)
        {
            return;
        }

        throw new CommandException(RemoteCommand.RemoteUnsetErrorMessage);
    }

    public static TgsRemote GetCurrentRemote(this ICommand command, IRemoteRegistry remotes)
    {
        command.FailIfNoRemote(remotes);
        return remotes.CurrentRemote!.Value;
    }

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
