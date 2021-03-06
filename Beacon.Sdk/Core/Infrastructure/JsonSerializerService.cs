namespace Beacon.Sdk.Core.Infrastructure
{
    using Domain.Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class JsonSerializerService : IJsonSerializerService
    {
        private static JsonSerializerSettings JsonSettings =>
            new()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new JsonConverter[] {new StringEnumConverter()}
            };

        public string Serialize(object model) => JsonConvert.SerializeObject(model, JsonSettings);

        public T Deserialize<T>(string input) => JsonConvert.DeserializeObject<T>(input, JsonSettings)!;
    }
}