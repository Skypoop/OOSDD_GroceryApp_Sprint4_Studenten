using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;

        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository,
            IGroceryListRepository groceryListRepository, IClientRepository clientRepository,
            IProductRepository productRepository)
        {
            _groceryListItemsRepository = groceryListItemsRepository;
            _groceryListRepository = groceryListRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }

        public List<BoughtProducts> Get(int? productId)
        {
            var boughtProducts = _groceryListRepository.GetAll()
                .Join(_clientRepository.GetAll(),
                    gl => gl.ClientId,
                    c => c.Id,
                    (gl, c) => new { GroceryList = gl, Client = c })
                .Join(_groceryListItemsRepository.GetAll(),
                    temp => temp.GroceryList.Id,
                    gli => gli.GroceryListId,
                    (temp, gli) => new { temp.GroceryList, temp.Client, GroceryListItem = gli })
                .Where(temp => productId == null || temp.GroceryListItem.ProductId == productId)
                .Join(_productRepository.GetAll(),
                    temp => temp.GroceryListItem.ProductId,
                    p => p.Id,
                    (temp, p) => new BoughtProducts(
                        temp.Client,
                        temp.GroceryList,
                        p))
                .ToList();

            return boughtProducts;
        }
    }
}