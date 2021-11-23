namespace Beacon.Sdk.Core.Infrastructure.Repositories
{
    public abstract class BaseLiteDbRepository
    {
        protected readonly string ConnectionString;

        protected BaseLiteDbRepository(RepositorySettings settings)
        {
            ConnectionString = settings.ConnectionString;
        }
    }
}