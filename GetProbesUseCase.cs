using System.Net.Http.Json;
using Converters;

record class ListProbesResponse(ProbeInfo[]? probes, string code, string? message);
record class ProbeInfo(string id, string name, string encoding);

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

    public async Task<Probe[]> Execute()
    {
        var response = await _http.Client.GetFromJsonAsync<ListProbesResponse>("probe");

        if (response?.code == "Success" && response.probes != null)
        {
            foreach (var probe in response.probes)
            {
                Console.WriteLine(probe.name);
            }

            return response.probes.Select(ConvertProbeInfo).ToArray();
        }
        else
        {
            throw new Exception("Failed to get probes");
        }
    }

    private Probe ConvertProbeInfo(ProbeInfo probesInfo)
    {
        var encoding = _encodings.TryGetValue(probesInfo.encoding, out var _encoding) ? _encoding : throw new ArgumentException("Invalid encoding", nameof(probesInfo.encoding));
        return new Probe(probesInfo.id, probesInfo.name, encoding);
    }
}