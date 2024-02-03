using System.Text.Json;
using System.Text;

public class HttpService(HttpClient client)
{
    public HttpClient Client { get; } = client;

    public async Task<string> Post(string requestUri, object? data = null)
    {
        HttpContent? content = null;

        if (data != null)
        {
            string body = JsonSerializer.Serialize(data);
            content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response = await Client.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();
 
        string responseContent = await response.Content.ReadAsStringAsync();

        return responseContent;
    }

    public async Task<T?> Post<T>(string requestUri, object? data = null)
    {
        string responseContent = await Post(requestUri, data);
        var response = JsonSerializer.Deserialize<T>(responseContent);

        return response;
    }
}
