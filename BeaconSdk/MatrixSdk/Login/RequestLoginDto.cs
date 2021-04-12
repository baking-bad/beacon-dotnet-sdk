namespace MatrixSdk.Login
{
    public class Identifier
    {
        public string type { get; set; }
        public string user { get; set; }
    }

    public class RequestLoginDto
    {
        public Identifier identifier { get; set; }
        public string password { get; set; }
        public string device_id { get; set; }
        public string type { get; set; }
    }
}