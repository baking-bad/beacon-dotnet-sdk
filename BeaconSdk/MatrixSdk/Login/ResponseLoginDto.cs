// ReSharper disable ClassNeverInstantiated.Global
namespace MatrixSdk.Login
{
    public class ResponseLoginDto
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string HomeServer { get; set; }
        public string DeviceId { get; set; }
    }
}