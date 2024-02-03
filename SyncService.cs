using Converters;
record class SyncResponse(string t1, string t2, string code, string? message);
public class SyncService(HttpService http)
{
    private readonly HttpService _http = http;
    private readonly TimeSpan _offsetThreshold = TimeSpan.FromSeconds(0.005);

    public async Task SyncClock(Clock clock)
    {
        int counter = 0;
        while (true)
        {
            var t0 = clock.Now;
            
            var response = await _http.Post<SyncResponse>($"probe/{clock.Probe.Id}/sync");
            
            if (response == null || response.code != "Success")
                throw new Exception("Failed to get timestamps");

            var t3 = clock.Now;

            var t1 = TimeConverter.Decode(response.t1, clock.Probe.Encoding);
            var t2 = TimeConverter.Decode(response.t2, clock.Probe.Encoding);

            var roundTrip = (t3 - t0) - (t2 - t1);
            var timeOffset = ((t1 - t0) + (t2 - t3)) / 2;

            clock.RoundTrip = roundTrip.Ticks;
            clock.TimeOffset += timeOffset;

            if (timeOffset.Duration() <= _offsetThreshold)
            {
                Console.WriteLine(clock.Probe.Name + " successfully synced");
                break;
            }
            
            counter++;

            if (counter > 30)
            {
                Console.WriteLine(clock.Probe.Name + " failed to sync");
                break;
            }
        }
    }
}
