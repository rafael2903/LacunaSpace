using Converters;

public class Probe(string id, string name, Encoding encoding)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Encoding Encoding { get; set; } = encoding;
}