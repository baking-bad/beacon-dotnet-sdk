namespace MatrixSdk
{
    using System.Threading.Tasks;

    public interface IMatrixClientStateRepository
    {
        Task<MatrixClientState> ReadState();

        Task CreateState(MatrixClientState state);

        Task UpdateState(MatrixClientState state);

        // Task<string> ReadAccessToken();
        // Task UpdateAccessToken(string accessToken);
        //
        // Task<string> ReadSyncNextBatch();
        //
        // Task UpdateSyncNextBatch(string nextBatch);
    }
}