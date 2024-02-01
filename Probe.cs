
using static TimeConverter;

public class Probe(string id, string name, string encoding)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Encoding Encoding { get; set; } = Encodings.ContainsKey(encoding) ? Encodings[encoding] : throw new ArgumentException("Invalid encoding", nameof(encoding));
}
