namespace MatrixSdk.Login
{
    public class ResponseLoginDto
    {
        public string user_id { get; set; }
        public string access_token { get; set; }
        public string home_server { get; set; }
        public string device_id { get; set; }
    }
}