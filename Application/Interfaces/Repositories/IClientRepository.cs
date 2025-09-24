


using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
        public Task<IEnumerable<Client>> AddClientsAsync(IEnumerable<Client> client);
        public Task<Client> AddClientAsync(Client client);
        public int GetCount();



    }
}
