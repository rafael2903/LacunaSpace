
using static TimeConverter;

public class Probe(string id, string name, string encoding)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Encoding Encoding { get; set; } = Encodings[encoding];
    public TimeSpan TimeOffset { get; set; }
    public TimeSpan RoundTrip { get; set; }
}
