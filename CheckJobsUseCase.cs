
using LacunaSpace;
using System.Text.Json;

public class CheckJobsUseCase(HttpService http)
{
    private readonly HttpService _http = http;

    public async Task Execute(List<Clock> clocks)
    {
        var checkJobUseCase = new CheckJobUseCase(http);

        while (true)
        {
            JobResponse? jobResponse = await http.Post<JobResponse>("job/take");

            if (jobResponse == null || jobResponse.code == "Error")
            {
                Console.WriteLine("Failed to take job");
                Console.WriteLine(JsonSerializer.Serialize(jobResponse));
                break;
            }
            else if (jobResponse.job == null)
            {
                Console.WriteLine("No jobs left");
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
