record class Job(string Id, string ProbeName);
record class JobResponse(Job? Job, string Code, string? Message);

public class CheckJobsUseCase(HttpService http)
{
    private readonly HttpService _http = http;

    public async Task Execute(List<Clock> clocks)
    {
        var checkJobUseCase = new CheckJobUseCase(_http);

        while (true)
        {
            JobResponse? jobResponse = await _http.Post<JobResponse>("job/take");

            if (jobResponse is null || jobResponse.Code == "Error")
            {
                Logger.LogError("Failed to take job");
                break;
            }
            else if (jobResponse.Job == null)
            {
                Logger.LogSuccess("No jobs left");
                break;
            }
            else if (jobResponse.Code == "Success")
            {
                Job job = jobResponse.Job;

                Console.WriteLine("Job taken - " + job.ProbeName);

                Clock clock = clocks.First(_clock => _clock.Probe.Name == job.ProbeName);

                await checkJobUseCase.Execute(job.Id, clock.Now, clock.Probe.Encoding, clock.RoundTrip);
            }
        }
    }
}
