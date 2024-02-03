﻿using System.Net.Http.Headers;
using System.Text.Json;

record class LoginResponse(string? accessToken, string code, string? message);
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

        var response = await _http.Post<LoginResponse>("start", data);

        if (response?.code == "Success")
        {
            Console.WriteLine("Successfully started test");
            _http.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.accessToken);
        }
        else
        {
            Console.WriteLine("Failed to start test");
            Environment.Exit(1);
        }
    }
}
