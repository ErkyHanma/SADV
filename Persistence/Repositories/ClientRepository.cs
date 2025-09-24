using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ClientRepository : IClientRepository
    {

        public readonly SADVContext _context;

        public ClientRepository(SADVContext context)
        {
            _context = context;
        }

        public Task<Client> AddClientAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            int clientCount = _context.Clients.Count();
            return clientCount;
        }

        public async Task<IEnumerable<Client>> AddClientsAsync(IEnumerable<Client> client)
        {

            await _context.Set<Client>().AddRangeAsync(client);
            await _context.SaveChangesAsync();
            return client;
        }
    }
}
