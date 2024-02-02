using System.Text.Json;
using Converters;

record class Response(string code, string? message);

public class CheckJobUseCase(HttpService http)
{
    private readonly HttpService _http = http;

    public async Task Execute(string jobId, DateTime probeNow, Encoding probeEncoding, long roundTrip)
    {
        var data = new
        {
            probeNow = TimeConverter.Encode(probeNow, probeEncoding),
            roundTrip
        };
        string body = JsonSerializer.Serialize(data);

        Response? response = await _http.Post<Response>($"job/{jobId}/check", body);

        if (response?.code == "Success")
        {
            Console.WriteLine("Job checked");
        }
        else if (response?.code == "Fail")
        {
            Console.WriteLine("failed");
        }
        else if (response?.code == "Done")
        {
            Console.WriteLine("Done");
        }
        else
        {
            Console.WriteLine("Failed to check job");
        }
    }
}
