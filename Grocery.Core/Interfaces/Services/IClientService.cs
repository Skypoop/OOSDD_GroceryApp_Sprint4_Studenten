using Grocery.Core.Models;

public interface IClientService
{
    Client? Get(string email);
    Client? Get(int id);
    List<Client> GetAll();

}