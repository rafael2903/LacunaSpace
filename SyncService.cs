using Converters;
record class SyncResponse(string T1, string T2, string Code, string? Message);
public class SyncService(HttpService http)
{
    private readonly HttpService _http = http;
    private readonly TimeSpan _offsetThreshold = TimeSpan.FromSeconds(0.005);
    private readonly TimeSpan _offsetThresholdTimeDilation = TimeSpan.FromSeconds(0.2);
    private readonly byte _maxSyncAttempts = 50;

    public async Task SyncClock(Clock clock)
    {
        bool hasTimeDilation = clock.Probe.TimeDilationFactor.HasValue;
        double timeDilationFactor = clock.Probe.TimeDilationFactor.GetValueOrDefault(1.0);
        byte counter = 0;

        while (true)
        {
            var t0 = clock.Now;
            var response = await _http.Post<SyncResponse>($"probe/{clock.Probe.Id}/sync");
            var t3 = clock.Now;

            clock.LastSync = DateTime.UtcNow;

            if (response?.Code == "ProbeUnreachable")
            {
                int _5Seconds = 5 * 1000;
                await Task.Delay(_5Seconds);
                continue;
            }

            if (response is null || response.Code != "Success")
            {
                Logger.LogError("Failed to get timestamps - " + clock.Probe.Name);
                break;
            }

            var t1 = TimeConverter.Decode(response.T1, clock.Probe.Encoding);
            var t2 = TimeConverter.Decode(response.T2, clock.Probe.Encoding);

            var roundTrip = (t3 - t0) - ((t2 - t1) * timeDilationFactor);
            var timeOffset = (t2 - t3) + (roundTrip / (2.0 * timeDilationFactor));

            clock.RoundTrip = roundTrip.Ticks;
            clock.TimeOffset += timeOffset;

            if (timeOffset.Duration() <= (hasTimeDilation ? _offsetThresholdTimeDilation : _offsetThreshold))
            {
                Logger.LogSuccess(clock.Probe.Name + " successfully syncronized");
                break;
            }

            counter++;

            if (counter > _maxSyncAttempts)
            {
                Logger.LogError(clock.Probe.Name + " failed to sync");
                break;
            }
        }
    }
}
