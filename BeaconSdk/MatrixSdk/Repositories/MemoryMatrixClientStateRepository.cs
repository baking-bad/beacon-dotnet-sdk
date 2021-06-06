namespace MatrixSdk
{
    using System.Threading.Tasks;

    internal class MemoryMatrixClientStateRepository : IMatrixClientStateRepository
    {
        private MatrixClientState matrixState;
        public Task<MatrixClientState> ReadState() => Task.FromResult(matrixState);
        public Task CreateState(MatrixClientState state)
        {
            matrixState = state;

            return Task.CompletedTask;
        }

        public Task UpdateState(MatrixClientState state)
        {
            matrixState = state;

            return Task.CompletedTask;
        }
    }
}