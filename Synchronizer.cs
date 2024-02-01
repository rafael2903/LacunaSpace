using System;
using System.Globalization;
using System.Text.Json;

record class SyncResponse(string t1, string t2, string code, string? message);
public class Synchronizer(Http http)
{
    private readonly Http _http = http;

    public async Task<(string t1, string t2)> GetTimestamps(string probeId)
    {
        var response = await _http.Post<SyncResponse>($"probe/{probeId}/sync");
        if (response != null && response.code == "Success")
        {
            return (response.t1,  response.t2);
        } else
        {
            throw new Exception("Failed to get timestamps");
        }
    }

}

                