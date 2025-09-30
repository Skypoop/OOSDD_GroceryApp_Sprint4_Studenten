using Grocery.Core.Models;

public interface IClientService
{
    Client? Get(string email);
    Client? Get(int id);
    List<Client> GetAll();
    Client? GetCurrentClient();
    void SetCurrentClient(Client client);
    void ClearCurrentClient();
}