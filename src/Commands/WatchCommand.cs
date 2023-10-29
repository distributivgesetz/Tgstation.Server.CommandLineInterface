namespace Tgstation.Server.CommandLineInterface.Commands;

using System.Text;
using Api.Models.Response;
using Client.Components;
using CliFx.Attributes;
using CliFx.Exceptions;
using Extensions;
using Instances;
using Middlewares;
using Models;
using Services;

[Command("watch", Description = "Display jobs in progress.")]
public class WatchCommand : BaseInstanceClientCommand
{
    public WatchCommand(ISessionManager sessions) : base(sessions)
    {
    }

    [CommandOption("focus", 'i', Converter = typeof(InstanceSelectorConverter),
        Description = "Display the jobs of specific instances. Values can be separated by semicolons.")]
    public InstanceSelector? FocusOn { get; init; }

    [CommandOption("run-once", 'o', Description = "Fetch current jobs once, then exit.")]
    public bool RunOnce { get; init; }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        try
        {
            context.Console.SetCursorVisible(false);

            var userInterruptToken = context.CancellationToken;
            var client = await this.Sessions.ResumeSession(userInterruptToken);

            var watcherErrorToken = new CancellationToken();

            using var watcherCancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource(userInterruptToken, watcherErrorToken);

            var token = watcherCancellationSource.Token;

            InstanceResponse[] availableInstances;

            if (this.FocusOn != null)
            {
                availableInstances = new[]
                {
                    await client.Instances.GetId(await this.SelectInstance(this.FocusOn, token), token)
                };
            }
            else
            {
                availableInstances = (await client.Instances.List(null, token)).ToArray();
            }

            var jobWatchers = availableInstances
                .Select(res => client.Instances.CreateClient(res))
                .Select(inst => new JobWatcher(inst.Jobs, this.RunOnce))
                .Select(watcher => (Watcher: watcher, Task: watcher.Run(token)))
                .ToList();

            const int maxColumns = 5;
            var tableWriter = new StringBuilder();
            var tableDeleter = new StringBuilder();

            var newLineCount = 0; // important for flushing the previous results

            while (!token.IsCancellationRequested &&
                   jobWatchers.Any(watcher =>
                       watcher.Watcher.Responses != null || watcher.Watcher.Responses?.Count != 0 ||
                       watcher.Task.IsCompleted))
            {
                tableWriter.Clear();
                tableDeleter.Clear();

                context.Console.CursorTop -= newLineCount;
                for (var i = 0; i < newLineCount; i++)
                {
                    tableDeleter.Append(new string(' ', context.Console.WindowWidth)).Append('\n');
                }

                await context.Console.WriteAsync(tableDeleter, token);
                context.Console.CursorTop -= newLineCount;

                tableWriter.AppendLine("-----");

                var jobs = jobWatchers
                    .SelectMany(watcher => watcher.Watcher.Responses?.ToArray() ?? Array.Empty<JobResponse>())
                    .ToArray();

                var currentJobIndex = 0;

                while (currentJobIndex < jobs.Length)
                {
                    if (jobWatchers.Any(watcher => watcher.Task.IsFaulted))
                    {
                        var failReason = new StringBuilder("Watching failed due to one or more errors.");

                        foreach (var error in jobWatchers
                                     .Select(w => w.Task)
                                     .Where(task => task.Exception != null)
                                     .Select(watcher => watcher.Exception!))
                        {
                            failReason.AppendLine(error.ToString());
                        }

                        watcherCancellationSource.Cancel();

                        throw new CommandException(failReason.ToString());
                    }

                    var rangeStart = currentJobIndex;
                    var rangeEnd = Math.Min(currentJobIndex + maxColumns, jobs.Length);
                    currentJobIndex = rangeEnd;

                    var tableJobs = jobs.Take(rangeStart..rangeEnd).ToArray();

                    var jobDescriptions = tableJobs.Select(job =>
                            (Id: job.Id.ToString()!, Description: job.Description?.ToString() ?? ">:3c", job.Progress))
                        .ToArray();

                    foreach (var jobDescription in jobDescriptions)
                    {
                        tableWriter.AppendLine(jobDescription.Id);
                        tableWriter.AppendLine(jobDescription.Description);
                        if (jobDescription.Progress != null)
                        {
                            tableWriter.AppendLine(GetProgressString(20, jobDescription.Progress.Value));
                        }

                        tableWriter.AppendLine("-----");
                    }


                    var output = tableWriter.ToString();
                    newLineCount = output.Count(c => c == '\n');
                    await context.Console.WriteAsync(output);
                }

                Thread.Sleep(100);
            }

            Task.WaitAll(jobWatchers.Select(w => w.Task).ToArray());

            return;

            static string GetProgressString(int totalLength, int progress)
            {
                var length = progress / (totalLength - 2);
                return $"[{new string('=', (int)Math.Floor((double)length) - 1)}>" +
                    $"{new string(' ', (int)(1 - Math.Floor((double)length)))}]";
            }
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
    }

    private sealed class JobWatcher
    {
        private readonly IJobsClient jobs;
        private readonly bool runOnce;

        public JobWatcher(IJobsClient jobs, bool runOnce)
        {
            this.jobs = jobs;
            this.runOnce = runOnce;
        }

        public IReadOnlyList<JobResponse>? Responses { get; private set; }

        public async Task Run(CancellationToken token)
        {
            try
            {
                do
                {
                    this.Responses = await this.jobs.List(null, token);
                } while (!this.runOnce);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
