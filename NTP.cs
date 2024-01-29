using System;
using System.Globalization;
using System.Text.Json;

record class SyncResponse(string t1, string t2, string code, string? message);
public class NTP(Http http)
{
    private readonly Http _http = http;
    private readonly TimeSpan _offsetThreshold = TimeSpan.FromSeconds(0.005);

    public async Task Sync(Probe probe)
    {
        while (true)
        {
            var t0 = DateTime.UtcNow;
            var response = await _http.Post<SyncResponse>($"probe/{probe.Id}/sync");
            var t3 = DateTime.UtcNow;

            if (response != null && response.code == "Success")
            {
                var t1 = TimeConverter.Decode(response.t1, probe.Encoding);
                var t2 = TimeConverter.Decode(response.t2, probe.Encoding);

                var roundTrip = (t3 - t0) - (t2 - t1);
                var timeOffset = ((t1 - t0) + (t2 - t3)) / 2;

                probe.RoundTrip = roundTrip;
                probe.TimeOffset += timeOffset;

                Console.WriteLine(timeOffset);

                if (timeOffset <= _offsetThreshold)
                {
                    Console.WriteLine(probe.Name + " successfully synced");
                    break;
                }
            }
        }
    }

}
