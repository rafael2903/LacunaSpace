using Converters;

record class Response(string Code, string? Message);

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

        Response? response = await _http.Post<Response>($"job/{jobId}/check", data);

        switch (response?.Code)
        {
            case "Success":
                Logger.LogSuccess("Job checked");
                break;
            case "Fail":
                Logger.LogError("Test failed");
                break;
            case "Done":
                Logger.LogSuccess("Done");
                break;
            default:
                Logger.LogError("Failed to check job");
                break;
        }
    }
}
