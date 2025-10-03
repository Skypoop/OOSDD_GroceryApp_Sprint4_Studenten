using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    /// <summary>
    /// Service for retrieving information about which clients bought which products.
    /// </summary>
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoughtProductsService"/> class.
        /// </summary>
        /// <param name="groceryListItemsRepository">Repository for grocery list items.</param>
        /// <param name="groceryListRepository">Repository for grocery lists.</param>
        /// <param name="clientRepository">Repository for clients.</param>
        /// <param name="productRepository">Repository for products.</param>
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository,
            IGroceryListRepository groceryListRepository, IClientRepository clientRepository,
            IProductRepository productRepository)
        {
            _groceryListItemsRepository = groceryListItemsRepository;
            _groceryListRepository = groceryListRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Gets a list of bought products filtered by a product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to filter by.</param>
        /// <returns>A list of <see cref="BoughtProducts"/> objects.</returns>
        public List<BoughtProducts> Get(int? productId)
        {
            var boughtProducts = new List<BoughtProducts>();
            var groceryListItems = _groceryListItemsRepository.GetAll();
            
            // Filter items by product ID.
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