namespace MatrixSdk
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class HttpClientExtensions
    {
        public static async Task<TResponse> PostJsonAsync<TResponse>(this HttpClient httpClient,
            string requestUri, object model)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };

            var json = JsonConvert.SerializeObject(model, settings);
            var content = new StringContent(json, Encoding.Default, "application/json");

            var response = await httpClient.PostAsync(requestUri, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new MatrixException(response.RequestMessage.RequestUri, 
                    json, result, response.StatusCode);

            return JsonConvert.DeserializeObject<TResponse>(result);
        }
    }
}