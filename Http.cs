using System;
using System.Text.Json;
using System.Text;

public class Http(HttpClient client)
{
    public HttpClient Client { get; } = client;
    //public async Task<T?> Post<T>(string requestUri, string? body = null)
    //{
    //    HttpContent? content = null;

    //    if (!string.IsNullOrEmpty(body))
    //        content = new StringContent(body, Encoding.UTF8, "application/json");

    //    var httpResponse = await _client.PostAsync(requestUri, content);
    //    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    //    Console.Write(responseContent);
    //    //var response = JsonSerializer.Deserialize<T>(responseContent);

    //    return responseContent;
    //}

    public async Task<string> Post(string requestUri, string? body = null)
    {
        HttpContent? content = null;

        if (!string.IsNullOrEmpty(body))
            content = new StringContent(body, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await Client.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();
 
        string responseContent = await response.Content.ReadAsStringAsync();

        return responseContent;
    }

    public async Task<T?> Post<T>(string requestUri, string? body = null)
    {
        string responseContent = await Post(requestUri, body);
        var response = JsonSerializer.Deserialize<T>(responseContent);

        return response;
    }
}
