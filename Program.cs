using System.Net.Http.Headers;

namespace LacunaSpace;

class Program
{
    public static async Task Main()
    {

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://luma.lacuna.cc/api/"),
            DefaultRequestHeaders =
            {
                Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                
            }
        };

        var http = new HttpService(httpClient);

        await new StartTestContextUseCase(http).Execute();
        var probes = await new GetProbesUseCase(http).Execute();
        var clocks = probes.Select(probe => new Clock(probe)).ToList();
        var syncService = new SyncService(http);
        //foreach (var clock in clocks)
        //{
        //    await syncService.SyncClock(clock);
        //}
        Task.WaitAll(clocks.Select(syncService.SyncClock).ToArray());
        await new CheckJobsUseCase(http).Execute(clocks);
    }
}
