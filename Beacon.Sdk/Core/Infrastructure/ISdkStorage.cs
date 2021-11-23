namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    public interface ISdkStorage
    {
        string? MatrixSelectedNode { get; set; }
    }
}