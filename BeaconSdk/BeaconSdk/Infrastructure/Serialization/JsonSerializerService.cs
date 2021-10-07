namespace BeaconSdk.Infrastructure.Serialization
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonSerializerService
    {
        private static JsonSerializerSettings JsonSettings =>
            new()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public string Serialize(object model) => JsonConvert.SerializeObject(model, JsonSettings);
    }
}