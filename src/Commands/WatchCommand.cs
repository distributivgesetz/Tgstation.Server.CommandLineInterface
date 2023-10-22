namespace Tgstation.Server.CommandLineInterface.Commands;

using Api.Models.Response;
using Client.Components;
using CliFx.Attributes;
using ConsoleTables;
using Extensions;
using Middlewares;
using Models;
using Services;
using Sessions;

[Command("watch", Description = "Display jobs in progress.")]
public class WatchCommand : BaseSessionCommand
{
    [CommandOption("focus", 'i', Converter = typeof(InstanceSelectorConverter),
        Description = "Display the jobs of specific instances. Values can be separated by semicolons.")]
    public InstanceSelector? FocusOn { get; init; }

    [CommandOption("run-once", 'o', Description = "Fetch current jobs once, then exit.")]
    public bool RunOnce { get; init; }

    public WatchCommand(ISessionManager sessions) : base(sessions)
    {
    }

    protected override async ValueTask RunCommandAsync(ICommandContext context)
    {
        var token = context.CancellationToken;
        var client = await this.Sessions.ResumeSession(token);

        var availableInstances = await client.Instances.List(null, token);

        var jobWatchers = availableInstances
            .Select(res => client.Instances.CreateClient(res))
            .Select(inst => new JobWatcher(inst.Jobs, this.RunOnce))
            .ToArray();

        var jobWatcherRuns = jobWatchers.Select(jobWatcher => jobWatcher.Run(token)).ToArray();

        const int maxColumns = 5;
        var tableWriter = new ConsoleTable();
        tableWriter.Configure(options => options.OutputTo = context.Console.Output);

        var stringLength = 0; // important for flushing the previous results

        while ((jobWatchers.Any(watcher => watcher.Responses != null || watcher.Responses?.Count != 0) || (jobWatcherRuns.Any(watcher => !watcher.IsCompleted) && token.IsCancellationRequested)))
        {
            for (var i = 0; i < stringLength; i++)
            {
                await context.Console.WriteAsync("\b \b");
            }

            var jobs = jobWatchers
                .SelectMany(watcher => watcher.Responses?.ToArray() ?? Array.Empty<JobResponse>())
                .ToArray();

            var currentJobIndex = 0;

            while (currentJobIndex < jobs.Length)
            {
                await context.Console.WriteLineAsync("in table");

                tableWriter.Rows.Clear();
                tableWriter.Columns.Clear();
                for (var i = 0; i < maxColumns; i++)
                {
                    currentJobIndex++;

                    var ids = jobs.Select(job => job.Id.ToString() ?? "test").ToArray();
                    var descriptions = jobs.Select(job => job.Description?.ToString() ?? ">:3c").ToArray();
                    await context.Console.WriteLineAsync("to add");
                    await context.Console.WriteLineAsync(ids.Length.ToString());
                    await context.Console.WriteLineAsync(descriptions.Length.ToString());

                    tableWriter.AddColumn(ids);
                    tableWriter.AddRow(descriptions);
                }
                var output = tableWriter.ToMinimalString();
                stringLength += output.Length + 1;
                await context.Console.WriteLineAsync(output);
            }

            Thread.Sleep(100);
        }

        Task.WaitAll(jobWatcherRuns);
    }

    private class JobWatcher
    {
        private readonly IJobsClient jobs;
        private readonly bool runOnce;

        public IReadOnlyList<JobResponse>? Responses { get; private set; }

        public JobWatcher(IJobsClient jobs, bool runOnce)
        {
            this.jobs = jobs;
            this.runOnce = runOnce;
        }

        public async Task Run(CancellationToken token)
        {
            do
            {
                try
                {
                    this.Responses = await this.jobs.List(null, token);
                }
                catch (TaskCanceledException)
                {
                    // fallthrough
                }
            } while (!token.IsCancellationRequested && !this.runOnce);
        }
    }
}
