using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    /// <summary>
    /// Service for managing grocery list items.
    /// </summary>
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository,
            IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            var groceryListItems = _groceriesRepository.GetAll();
            // Populate product details for the retrieved items
            FillProducts(groceryListItems);
            return groceryListItems;
        }

        /// <summary>
        /// Gets all grocery list items for a specific grocery list.
        /// Note: This assumes the repository can filter by groceryListId at the data source, which is more efficient.
        /// </summary>
        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            var groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillProducts(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            return _groceriesRepository.Delete(item);
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        /// <summary>
        /// Gets the top X best-selling products.
        /// </summary>
        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            var groceryListItems = _groceriesRepository.GetAll();
            var productSummaries = CalculateTopSellingProducts(groceryListItems, topX);
            return CreateBestSellingProductsList(productSummaries);
        }

        private List<BestSellingProducts> CreateBestSellingProductsList(
            IEnumerable<(int ProductId, int TotalSold)> productSummaries)
        {
            var productIds = productSummaries.Select(s => s.ProductId).ToList();
            // Fetch all required products in a single query to avoid the N+1 problem.
            var products = _productRepository.GetAll()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionary(p => p.Id);

            int rank = 1;
            return productSummaries.Select(summary =>
            {
                var product = products.GetValueOrDefault(summary.ProductId, new Product(0, "Unknown", 0));
                return new BestSellingProducts(
                    summary.ProductId,
                    product.Name,
                    product.Stock,
                    summary.TotalSold,
                    rank++
                );
            }).ToList();
        }

        private IEnumerable<(int ProductId, int TotalSold)> CalculateTopSellingProducts(
            List<GroceryListItem> groceryListItems, int topX)
        {
            return groceryListItems
                .GroupBy(item => item.ProductId)
                .Select(group => (
                    ProductId: group.Key,
                    TotalSold: group.Sum(item => item.Amount)
                ))
                .OrderByDescending(summary => summary.TotalSold)
                .Take(topX);
        }

        /// <summary>
        /// Efficiently populates the Product property for a list of GroceryListItems.
        /// </summary>
        private void FillProducts(List<GroceryListItem> groceryListItems)
        {
            if (!groceryListItems.Any()) return;

            var productIds = groceryListItems.Select(g => g.ProductId).Distinct().ToList();
            
            // Fetch all needed products
            var products = _productRepository.GetAll()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionary(p => p.Id);

            foreach (var item in groceryListItems)
            {
                item.Product = products.GetValueOrDefault(item.ProductId, new Product(0, "Unknown", 0));
            }
        }
    }
}
