public class Clock(Probe probe)
{
    public Probe Probe { get; set; } = probe;
    public TimeSpan TimeOffset { get; set; } = TimeSpan.Zero;
    public long RoundTrip { get; set; } = 0;
    public DateTime? LastSync { get; set; }

    public DateTime Now
    {
        get
        {
            if (LastSync is null || Probe?.TimeDilationFactor is null)
                return DateTime.UtcNow + TimeOffset;
            else
            {
                var elapsedTime = TimeSpan.FromTicks((long)((DateTime.UtcNow.Ticks - LastSync.Value.Ticks) / Probe.TimeDilationFactor.Value));
                return LastSync.Value + elapsedTime + TimeOffset;
            }
        }
    }
}
