namespace MatrixSdk.Dto.Login
{
    public record Identifier(string Type, string User)
    {
        public string Type { get; } = Type;
        public string User { get; } = User;
    }
}