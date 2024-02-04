
using System.Text.Json;

public class Clock(Probe probe)
{
    public Probe Probe { get; set; } = probe;
    public TimeSpan TimeOffset { get; set; } = TimeSpan.Zero;
    public long RoundTrip { get; set; } = 0;
    public DateTime? LastSync { get; set; } = null;

    public DateTime Now2 => DateTime.UtcNow + TimeOffset;
    public DateTime Now
    {
        get
        {
            //Console.WriteLine("LastSync: " + LastSync.HasValue ?? LastSync.Value);
            if (!LastSync.HasValue || Probe?.TimeDilationFactor is null)
            {
                //Console.WriteLine(JsonSerializer.Serialize(Probe));
                //Console.WriteLine("LastSync.HasValue: " + LastSync.HasValue);
                //if (LastSync.HasValue)
                //{
                //    Console.WriteLine("LastSync: " + LastSync.Value);
                //}
                //Console.WriteLine("TimeDilationFactor: " + Probe?.TimeDilationFactor == null) ;
                return DateTime.UtcNow + TimeOffset;
            }
            else
            {
                var data = LastSync.Value + TimeOffset + TimeSpan.FromTicks((long)((DateTime.UtcNow.Ticks - LastSync.Value.Ticks) / Probe.TimeDilationFactor.Value));
                //Console.WriteLine("Clock.Now: " + data);
                //Console.WriteLine("LastSync: " + LastSync.Value);
                //Console.WriteLine("TimeOffset: " + TimeOffset);
                //Console.WriteLine("DateTime.UtcNow: " + DateTime.UtcNow);
                //Console.WriteLine("Probe.TimeDilationFactor: " + Probe.TimeDilationFactor);
                //Console.WriteLine("Time difference: " + (DateTime.UtcNow.Ticks - LastSync.Value.Ticks));

                return data;
            }
                //return DateTime.UtcNow + TimeOffset + TimeSpan.FromTicks(Double.Round(LastSync.Ticks / Probe.TimeDilationFactor));
        }
    }
}
