namespace Beacon.Sdk.Core.Infrastructure.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonSerializerService
    {
        private static JsonSerializerSettings JsonSettings =>
            new()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

        public string Serialize(object model) => JsonConvert.SerializeObject(model, JsonSettings);

        public T Deserialize<T>(string input) => JsonConvert.DeserializeObject<T>(input, JsonSettings)!;
    }
}