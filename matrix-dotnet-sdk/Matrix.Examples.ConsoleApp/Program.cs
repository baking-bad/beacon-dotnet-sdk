namespace Matrix.Examples.ConsoleApp
{
    using System.Threading.Tasks;

    internal class Program
    {
        private static readonly SimpleExample SimpleExample = new();

        private static async Task<int> Main(string[] args)
        {
            await SimpleExample.Run();

            return 0;
        }
    }
}