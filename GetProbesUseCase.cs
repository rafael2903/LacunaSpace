using System.Net.Http.Json;
using Converters;

record class ListProbesResponse(ProbeInfo[]? Probes, string Code, string? Message);
record class ProbeInfo(string Id, string Name, string Encoding, double? TimeDilationFactor);

public class GetProbesUseCase(HttpService http)
{
    private readonly HttpService _http = http;

    private static readonly Dictionary<string, Encoding> _encodings = new()
    {
        { "Iso8601", Encoding.ISO8601 },
        { "Ticks", Encoding.Ticks },
        { "TicksBinaryBigEndian", Encoding.TicksBigEndian },
        { "TicksBinary", Encoding.TicksLittleEndian }
    };

    private Probe ConvertProbeInfo(ProbeInfo probesInfo)
    {
        var encoding = _encodings.TryGetValue(probesInfo.Encoding, out var _encoding) ? _encoding : throw new ArgumentException("Invalid encoding", nameof(probesInfo));

        return new Probe(probesInfo.Id, probesInfo.Name, encoding, probesInfo.TimeDilationFactor);
    }

    public async Task<Probe[]> Execute()
    {
        var response = await _http.Client.GetFromJsonAsync<ListProbesResponse>("probe");

        if (response?.Code == "Success" && response.Probes != null)
        {
            Console.Write("Probes: ");
            Console.WriteLine(string.Join(", ", response.Probes.Select(probe => probe.Name)));

            return response.Probes.Select(ConvertProbeInfo).ToArray();
        }
        else throw new Exception("Failed to get probes");
    }
}