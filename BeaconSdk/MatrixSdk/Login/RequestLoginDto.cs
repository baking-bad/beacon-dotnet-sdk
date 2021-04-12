namespace MatrixSdk.Login
{
    public class Identifier
    {
        public string Type { get; set; }
        public string User { get; set; }
    }

    public class RequestLoginDto
    {
        public Identifier Identifier { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public string Type { get; set; }
    }
}