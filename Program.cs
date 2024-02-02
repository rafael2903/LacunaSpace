namespace LacunaSpace;
record class Job(string id, string probeName);
record class JobResponse(Job? job, string code, string? message);

class Program
{
    public static async Task Main()
    {

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://luma.lacuna.cc/api/")
        };

        var http = new HttpService(httpClient);

        await new StartTestContextUseCase(http).Execute();
        var probes = await new GetProbesUseCase(http).Execute();
        var clocks = probes.Select(probe => new Clock(probe)).ToList();
        var synchronizer = new Synchronizer(http);
        Task.WaitAll(clocks.Select(clock => clock.Sync(synchronizer)).ToArray());
        await new CheckJobsUseCase(http).Execute(clocks);
    }
}
