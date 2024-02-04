namespace LacunaSpace;

class Program
{
    public static async Task Main()
    {

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://luma.lacuna.cc/api/")
        };

        var http = new HttpService(httpClient);

        try
        {
            await new StartTestContextUseCase(http).Execute();

            var probes = await new GetProbesUseCase(http).Execute();

            var clocks = probes.Select(probe => new Clock(probe)).ToList();

            var syncService = new SyncService(http);

            Logger.LogInfo("Syncronizing clocks");

            Task.WaitAll(clocks.Select(syncService.SyncClock).ToArray());

            await new CheckJobsUseCase(http).Execute(clocks);
        }
        catch (Exception e) { Logger.LogError(e.Message); }
    }
}
