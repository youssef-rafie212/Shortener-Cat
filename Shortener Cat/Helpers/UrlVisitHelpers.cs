using System.Text.Json;

namespace Shortener_Cat.Helpers
{
    public static class UrlVisitHelpers
    {
        public static async Task<string?> GetCountryNameFromIpAddress(string? ip)
        {
            if (ip == null) return null;

            using var http = new HttpClient();
            var response = await http.GetStringAsync($"http://ip-api.com/json/{ip}");

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            if (root.GetProperty("status").GetString() == "success")
            {
                return root.GetProperty("country").GetString();
            }

            return null;
        }
    }
}
