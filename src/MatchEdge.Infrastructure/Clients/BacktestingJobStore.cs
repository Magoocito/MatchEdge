using System.Collections.Concurrent;

namespace MatchEdge.Infrastructure.Clients;

public class BacktestingJobStore
{
    private readonly ConcurrentDictionary<string, BacktestingJob> _jobs = new();

    public BacktestingJob CreateJob()
    {
        var job = new BacktestingJob();
        _jobs[job.JobId] = job;
        return job;
    }

    public BacktestingJob? GetJob(string jobId)
    {
        _jobs.TryGetValue(jobId, out var job);
        return job;
    }

    public IReadOnlyList<BacktestingJob> GetAllJobs()
    {
        return _jobs.Values.OrderByDescending(j => j.StartedAt).ToList();
    }
}
