using System.Collections.Generic;

namespace Beacon.Sdk.Core.Domain.Interfaces.Data
{
    using System.Threading.Tasks;
    using Entities;

    public interface IPeerRepository
    {
        Task<Peer> CreateAsync(Peer peer);
        Task<Peer?> TryReadAsync(string senderUserId);
        Task<List<Peer>> GetAll();
    }
}