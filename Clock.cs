using System;
using System.Diagnostics.Metrics;
using static System.Net.WebRequestMethods;

public class Clock(Probe probe)
{
    public Probe Probe { get; set;} = probe;
    public TimeSpan TimeOffset { get; set; } = TimeSpan.Zero;
    public long RoundTrip { get; set; } = 0;

    private readonly TimeSpan _offsetThreshold = TimeSpan.FromSeconds(0.005);

    public DateTime CurrentTime => (DateTime.UtcNow + TimeOffset);
    public async Task Sync(Synchronizer synchronizer)
    {
        int counter = 0;
        while (true)
        {
            var t0 = CurrentTime;
            var (t1Str, t2Str) = await synchronizer.GetTimestamps(Probe.Id);
            var t3 = CurrentTime;

            counter++;
            var t1 = TimeConverter.Decode(t1Str, probe.Encoding);
            var t2 = TimeConverter.Decode(t2Str, probe.Encoding);

            var roundTrip = (t3 - t0) - (t2 - t1);
            var timeOffset = ((t1 - t0) + (t2 - t3)) / 2;

            RoundTrip = roundTrip.Ticks;
            TimeOffset += timeOffset;

            Console.WriteLine(timeOffset);

            if (timeOffset.Duration() <= _offsetThreshold)
            {
                Console.WriteLine(probe.Name + " successfully synced");
                break;
            }

            if (counter > 20)
            {
                Console.WriteLine(probe.Name + " failed to sync");
                break;
            }
        }
    }
}
