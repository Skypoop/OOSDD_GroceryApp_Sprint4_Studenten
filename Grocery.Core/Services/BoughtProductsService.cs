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
            var boughtProducts = new List<BoughtProducts>();
            var groceryListItems = _groceryListItemsRepository.GetAll();
            groceryListItems = groceryListItems.Where(gli => gli.ProductId == productId).ToList();

            foreach (var gli in groceryListItems)
            {
                var groceryList = _groceryListRepository.Get(gli.GroceryListId);
                if (groceryList == null) continue;
                var client = _clientRepository.Get(groceryList.ClientId);
                var product = _productRepository.Get(gli.ProductId);
                if (client != null && product != null)
                {
                    boughtProducts.Add(new BoughtProducts(client, groceryList, product));
                }
            }

            return boughtProducts;
        }
    }
}