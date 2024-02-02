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

        var checkJobUseCase = new CheckJobUseCase(http);

        while (true)
        {
            JobResponse? jobResponse = await http.Post<JobResponse>("job/take");

            if (jobResponse == null || jobResponse.code == "Error")
            {
                Console.WriteLine("Failed to take job");
                break;
            }
            else if (jobResponse.job == null)
            {
                Console.WriteLine("No more jobs");
                break;
            }
            else if (jobResponse.code == "Success")
            {
                Job job = jobResponse.job;
                Console.WriteLine("Job taken - " + job.probeName);

                Clock clock = clocks.First(_clock => _clock.Probe.Name == job.probeName);

                await checkJobUseCase.Execute(job.id, clock.Now, clock.Probe.Encoding, clock.RoundTrip);
            }   
        }
    }
}
