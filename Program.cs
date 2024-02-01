using System.Buffers.Binary;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace LacunaSpace;
record class LoginResponse(string? accessToken, string code, string? message);
record class ListProbesResponse(ProbeInfo[]? probes, string code, string? message);
record class ProbeInfo(string id, string name, string encoding);
record class Job(string id, string probeName);
record class JobResponse(Job? job, string code, string? message);
record class Response(string code, string? message);

class Program
{
    public static async Task Main()
    {
        List<Clock> clocks;

        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://luma.lacuna.cc/api/");
        var http = new Http(httpClient);

        await StartTestContext(http);
        var probes = await GetProbes(http);

        clocks = probes.Select(probe => (new Clock(new Probe(probe.id, probe.name, probe.encoding)))).ToList();
        var synchronizer = new Synchronizer(http);

        foreach (var clock in clocks)
        {
            await clock.Sync(synchronizer);
        }

        while (true)
        {
            JobResponse? jobResponse = await http.Post<JobResponse>("job/take");
            if (jobResponse?.code == "Success" && jobResponse.job != null)
            {
                Job job = jobResponse.job;
                Console.WriteLine("Job taken");
                Console.WriteLine(job.probeName);

                //Probe probe = clocks.FirstOrDefault(clock => clock.Probe.Name == job.probeName)?.Probe;
                Clock clock = (from _clock in clocks
                                      where _clock.Probe.Name == job.probeName
                                      select _clock
                             ).First();

                Dictionary<string, object> data = new()
                {
                    { "probeNow",  TimeConverter.Encode(clock.CurrentTime, clock.Probe.Encoding)},
                    { "roundTrip", clock.RoundTrip }
                };
                string body = JsonSerializer.Serialize(data);
                
                Response? response = await http.Post<Response>($"job/{job.id}/check", body);
                if (response?.code == "Success")
                {
                    Console.WriteLine("Job checked");
                }
                else if (response?.code == "Fail")
                {
                    Console.WriteLine("failed");
                    break;
                }
                else if (response?.code == "Done")
                {
                    Console.WriteLine("Done");
                    break;
                }
                else
                {
                    Console.WriteLine("Failed to check job");
                    break;
                }
            }
            else
            {
                Console.WriteLine("Failed to take job");
            }
            
            //await Task.Delay(1000);
        }
    }

    public static async Task StartTestContext(Http http)
    {
        Dictionary<string, string> data = new()
        {
            { "username", "Rafael Rodrigues" },
            { "email", "rafaelrodrigues2903@gmail.com" }
        };
        string body = JsonSerializer.Serialize(data);

        var response = await http.Post<LoginResponse>("start", body);

        if (response?.code == "Success")
        {
            Console.WriteLine("Successfully logged in");
            http.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.accessToken);
        }
    }

    public static async Task<ProbeInfo[]> GetProbes(Http http)
    {
        var response = await http.Client.GetFromJsonAsync<ListProbesResponse>("probe");

        if (response?.code == "Success" && response.probes != null)
        {
            foreach (var probe in response.probes)
            {
                Console.WriteLine(probe.name);
                Console.WriteLine(probe.id);
                Console.WriteLine(probe.encoding);
            }
            return response.probes;
        }
        else
        {
            throw new Exception("Failed to get probes");
        }
    }
}
