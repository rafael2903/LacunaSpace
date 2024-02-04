using System.Text.Json;
using System.Text;

public class HttpService(HttpClient client)
{
    public HttpClient Client { get; } = client;

    public async Task<string> Post(string requestUri, object? data = null)
    {
        HttpContent? content = null;

        if (data is not null)
        {
            string body = JsonSerializer.Serialize(data);
            content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        var response = await Client.PostAsync(requestUri, content);

        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();

        return responseContent;
    }

    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<T?> Post<T>(string requestUri, object? data = null)
    {
        string responseContent = await Post(requestUri, data);
        return JsonSerializer.Deserialize<T>(responseContent, jsonOptions);
    }
}
