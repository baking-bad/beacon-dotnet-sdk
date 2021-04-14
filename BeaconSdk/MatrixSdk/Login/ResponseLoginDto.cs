namespace MatrixSdk.Login
{
    public record ResponseLoginDto(string UserId, string AccessToken, string HomeServer, string DeviceId);
}