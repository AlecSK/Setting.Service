using System;
using System.Net.Http;

namespace Setting.Service.TestClient
{
    public static class Program
    {
        static void Main()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };

            using (var httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("user1:password1");
                string val = Convert.ToBase64String(plainTextBytes);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + val);

                var client = new OpenApiClient("https://localhost:44359/", httpClient);

                var result = client.GetModulesAsync().Result;
                foreach (var module in result)
                {
                    Console.WriteLine($"{module.Id} : {module.SystemName}");
                }
            }

            Console.ReadLine();
        }
    }
}
