using System;
using System.Net.Http.Json;
record class Response(string code, string? message);

class UnauthorizedHandler : DelegatingHandler
{
    public UnauthorizedHandler() : base(new HttpClientHandler())
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        try
        {
            Response? responseContent = await response.Content.ReadFromJsonAsync<Response>();

            if (responseContent?.code == "Unauthorized")
            {
                Console.WriteLine("Unauthorized Error. Performing custom handling...");

            }
        }
        catch (Exception)
        {
        }

        return response;
    }
}