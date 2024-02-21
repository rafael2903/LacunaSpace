using LacunaSpace;
using System.Net.Http.Headers;
using static LacunaSpace.Program;
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
        var requestUri = Globals.UseV2Api ? "start/2" : "start";
        var response = await _http.Post<LoginResponse>(requestUri, data);

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
