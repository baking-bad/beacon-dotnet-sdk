namespace MatrixSdk
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class HttpClientExtensions
    {
        public static async Task<TResponse> PostAsync<TRequest, TResponse>(this HttpClient httpClient,
            string requestUri, TRequest model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.Default, "application/json");

            var response = await httpClient.PostAsync(requestUri, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Matrix API error. Status: {response.StatusCode}, " +
                                    $"data: {result}"); // Todo: implement custom Exception
            
            return JsonSerializer.Deserialize<TResponse>(result);
        }
    }
}