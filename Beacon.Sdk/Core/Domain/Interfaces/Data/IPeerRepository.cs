namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using Entities;

    public interface IPeerRepository
    {
        Task<Peer> Create(Peer peer);

        Task<Peer?> TryRead(string senderUserId);
    }
}