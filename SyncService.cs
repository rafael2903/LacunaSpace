using Converters;
using System.Text.Json;
record class SyncResponse(string t1, string t2, string code, string? message);
public class SyncService(HttpService http)
{
    private readonly HttpService _http = http;
    private readonly TimeSpan _offsetThreshold = TimeSpan.FromSeconds(0.005);
    private readonly TimeSpan _offsetThresholdForTimeDilation = TimeSpan.FromSeconds(0.1);
    private readonly int _maxSyncAttempts = 50;

    public async Task SyncClock(Clock clock)
    {
        int counter = 0;
        while (true)
        {
            var t0 = clock.Now;
            
            var response = await _http.Post<SyncResponse>($"probe/{clock.Probe.Id}/sync");
            
            var t3 = clock.Now;
            clock.LastSync = DateTime.UtcNow;

            if (response?.code == "ProbeUnreachable")
            {
                int _5Seconds = 5 * 1000;
                await Task.Delay(_5Seconds);
                continue;
            }

            if (response == null || response.code != "Success")
            {
                Console.WriteLine(JsonSerializer.Serialize(response));
                throw new Exception("Failed to get timestamps");
            }

            var t1 = TimeConverter.Decode(response.t1, clock.Probe.Encoding);
            var t2 = TimeConverter.Decode(response.t2, clock.Probe.Encoding);
            
            var roundTrip = (t3 - t0) - ((t2 - t1) * clock.Probe.TimeDilationFactor.GetValueOrDefault(1.0));
            var roundTrip2 = (t3 - t0) - (t2 - t1);

            //Console.WriteLine("Old t2: " + response.t2);
            //if (clock.Probe.TimeDilationFactor is not null)
            //{
            //    //Console.WriteLine("Time dilation effect: " + (t2 - (t1 + (t2 - t1) * (1.0 + clock.Probe.TimeDilationFactor.Value))).TotalMilliseconds);
            //    t2 = t1 + (t2 - t1) * (1.0 + clock.Probe.TimeDilationFactor.Value);
            //}
            //Console.WriteLine("New t2: " + t2);

            //var timeOffset = ((t1 - t0) + (t2 - t3)) / 2;
            //var timeOffset = ((t2 - t3) - (roundTrip / 2));
            var timeOffset = ((roundTrip / (2.0 * clock.Probe.TimeDilationFactor.GetValueOrDefault(1.0))) + (t2 - t3));

            clock.RoundTrip = roundTrip.Ticks;
            clock.TimeOffset += timeOffset;

            if (timeOffset.Duration() <= (clock.Probe.TimeDilationFactor.HasValue ? _offsetThresholdForTimeDilation : _offsetThreshold))
            {
                Console.WriteLine(clock.Probe.Name + " - "+ clock.Probe.TimeDilationFactor.GetValueOrDefault(1.0) + " successfully synced");
                break;
            }
            
            counter++;


            if (counter > _maxSyncAttempts)
            {
                Console.WriteLine(clock.Probe.Name + " failed to sync");
                break;
            }
        }
    }
}
