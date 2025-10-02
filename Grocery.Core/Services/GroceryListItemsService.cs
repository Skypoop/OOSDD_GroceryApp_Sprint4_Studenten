using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
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
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems =
                _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
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

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            var groceryListItems = _groceriesRepository.GetAll();
            var productSummaries = CalculateTopSellingProducts(groceryListItems, topX);
            return CreateBestSellingProductsList(productSummaries);
        }

        private List<BestSellingProducts> CreateBestSellingProductsList(
            IEnumerable<(int ProductId, int TotalSold)> productSummaries)
        {
            var bestSellingProducts = new List<BestSellingProducts>();
            int rank = 1;
            foreach (var summary in productSummaries)
            {
                var product = _productRepository.Get(summary.ProductId) ?? new Product(0, "Unknown", 0);

                bestSellingProducts.Add(new BestSellingProducts(
                    summary.ProductId,
                    product.Name,
                    product.Stock,
                    summary.TotalSold,
                    rank
                ));
                rank++;
            }

            return bestSellingProducts;
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

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}