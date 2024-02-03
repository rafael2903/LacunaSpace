using Converters;

public class Clock(Probe probe)
{
    public Probe Probe { get; set;} = probe;
    public TimeSpan TimeOffset { get; set; } = TimeSpan.Zero;
    public long RoundTrip { get; set; } = 0;
    public DateTime Now => (DateTime.UtcNow + TimeOffset);
}
