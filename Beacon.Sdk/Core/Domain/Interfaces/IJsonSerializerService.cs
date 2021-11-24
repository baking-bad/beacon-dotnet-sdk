namespace Beacon.Sdk.Core.Domain.Interfaces
{
    public interface IJsonSerializerService
    {
        string Serialize(object model);
        T Deserialize<T>(string input);
    }
}