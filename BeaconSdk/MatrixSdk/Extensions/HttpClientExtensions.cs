namespace MatrixSdk.Extensions
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    internal static class HttpClientExtensions
    {
        private static JsonSerializerSettings GetJsonSettings()
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,

                //MatrixClientService.CreateRoomAsync not working with null in Json
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        public static async Task PostAsync(this HttpClient httpClient,
            string requestUri, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsync(requestUri, null, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new MatrixApiException(response.RequestMessage.RequestUri,
                    null, null, response.StatusCode);
        }

        // Todo: Refactor
        // See: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0#httpclient-and-httpcontent-extension-methods
        public static async Task<TResponse> PostAsJsonAsync<TResponse>(this HttpClient httpClient,
            string requestUri, object model, CancellationToken cancellationToken)
        {
            var settings = GetJsonSettings();

            var json = JsonConvert.SerializeObject(model, settings);
            var content = new StringContent(json, Encoding.Default, "application/json");

            var response = await httpClient.PostAsync(requestUri, content, cancellationToken);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new MatrixApiException(response.RequestMessage.RequestUri,
                    json, result, response.StatusCode);

            return JsonConvert.DeserializeObject<TResponse>(result, settings)!;
        }

        // Todo: Refactor
        // See: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0#httpclient-and-httpcontent-extension-methods
        public static async Task<TResponse> PutAsJsonAsync<TResponse>(this HttpClient httpClient,
            string requestUri, object model)
        {
            var settings = GetJsonSettings();

            var json = JsonConvert.SerializeObject(model, settings);
            var content = new StringContent(json, Encoding.Default, "application/json");

            var response = await httpClient.PutAsync(requestUri, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new MatrixApiException(response.RequestMessage.RequestUri,
                    json, result, response.StatusCode);

            return JsonConvert.DeserializeObject<TResponse>(result, settings)!;
        }

        public static async Task<TResponse> GetAsJsonAsync<TResponse>(this HttpClient httpClient,
            string requestUri, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync(requestUri, cancellationToken);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new MatrixApiException(response.RequestMessage.RequestUri,
                    null, result, response.StatusCode);

            return JsonConvert.DeserializeObject<TResponse>(result, GetJsonSettings())!;
        }

        public static void AddBearerToken(this HttpClient httpClient, string bearer)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearer);
        }
    }
}