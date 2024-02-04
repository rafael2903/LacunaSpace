using Converters;

public class Probe(string id, string name, Encoding encoding, double? timeDilationFactor)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Encoding Encoding { get; set; } = encoding;
    public double? TimeDilationFactor { get; set; } = timeDilationFactor;
}