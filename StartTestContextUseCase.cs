using System.Net.Http.Headers;

record class LoginResponse(string? AccessToken, string Code, string? Message);
public class StartTestContextUseCase(HttpService http)
{
    private readonly HttpService _http = http;

    public async Task Execute()
    {
        var data = new
        {
            username = "Rafael Rodrigues",
            email = "rafaelrodrigues2903@gmail.com"
        };

        var response = await _http.Post<LoginResponse>("start/2", data);

        if (response?.Code == "Success")
        {
            Logger.LogSuccess("Successfully started test");
            _http.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
        }
        else
        {
            throw new Exception("Failed to start test");
        }
    }
}
