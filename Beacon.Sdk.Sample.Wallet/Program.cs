namespace Beacon.Sdk.Sample.Wallet
{
    using System.Threading.Tasks;

    internal class Program
    {
        private static readonly Sample Sample = new();

        private static async Task<int> Main(string[] args)
        {
            await Sample.Run();
            System.Console.ReadLine();
            return 0;
        }
    }
}