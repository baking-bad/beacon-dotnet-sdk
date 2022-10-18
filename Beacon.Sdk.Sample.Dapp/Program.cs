namespace Beacon.Sdk.Sample.Dapp
{
    internal class Program
    {
        private static readonly Sample Sample = new();

        private static async Task<int> Main(string[] args)
        {
            await Sample.Run();
            return 0;
        }
    }   
}