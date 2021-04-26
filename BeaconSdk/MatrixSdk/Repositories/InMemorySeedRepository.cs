namespace MatrixSdk.Repositories
{
    using System;
    using System.Threading.Tasks;

    public interface ISeedRepository
    {
        Task<Guid> GetSeed();
    }

    public sealed class InMemorySeedRepository : ISeedRepository
    {
        private static readonly Guid Seed = Guid.NewGuid(); // Todo: refactor

        public Task<Guid> GetSeed() => Task.FromResult(Seed);
    }
}