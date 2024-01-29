using System.Buffers.Binary;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace LacunaSpace;
record class LoginResponse(string? accessToken, string code, string? message);
record class ListProbesResponse(ProbeInfo[]? probes, string code, string? message);
record class ProbeInfo(string id, string name, string encoding);
class Program
{
    public static async Task Main(string[] args)
    {
        //string dateString;
        //DateTimeOffset offsetDate;

        //// String with date only
        //dateString = "2023-06-03T12:57:27.6003807+00:00"; // ticks based on the ISO datetime string format: yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz
        //offsetDate = DateTimeOffset.ParseExact(dateString, "o", CultureInfo.InvariantCulture);
        //Console.WriteLine(offsetDate.ToString());
        //Console.WriteLine("638213938476003807"); // ticks long value string
        //Console.WriteLine(offsetDate.Ticks);
        ////Console.WriteLine(DateTimeStyles.RoundtripKind);
        //Console.WriteLine(BitConverter.IsLittleEndian);
        //Console.WriteLine(BitConverter.ToInt64(Convert.FromBase64String("37GQFTJk2wg="))); // ticks long bytes (little-endian) Base64 string
        //Console.WriteLine(BinaryPrimitives.ReverseEndianness(BitConverter.ToInt64(Convert.FromBase64String("CNtkMhWQsd8=")))); // ticks long bytes (big-endian) Base64 string

        List<Probe> probes;

        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://luma.lacuna.cc/api/");
        var http = new Http(httpClient);

        await StartTestContext(http);
        await GetProbes(http);
        //var ntp = new NTP(http);

        //foreach (var probe in probes)
        //{
        //    await ntp.Sync(probe);
        //}
    }

    public static async Task StartTestContext(Http http)
    {
        Dictionary<string, string> data = new()
        {
            { "username", "Rafael Rodrigues" },
            { "email", "rafaelrodrigues2903@gmail.com" }
        };
        string body = JsonSerializer.Serialize(data);

        //var response = await Post<LoginResponse>(client, "users/login", body);
        var response = await http.Post<LoginResponse>("start", body);

        if (response?.code == "Success")
        {
            Console.WriteLine("Successfully logged in");
            http.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.accessToken);
        }
    }

    public static async Task GetProbes(Http http)
    {
        var response = await http.Client.GetFromJsonAsync<ListProbesResponse>("probe");

        if (response?.code == "Success" && response.probes != null)
        {
            foreach (var probe in response.probes)
            {
                Console.WriteLine(probe.name);
            }
        }
    }
}
