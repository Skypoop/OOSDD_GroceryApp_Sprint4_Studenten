using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty] Product? selectedProduct;

        public bool IsProductSelected => SelectedProduct != null;

        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; } = [];
        public ObservableCollection<Product> Products { get; set; }

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
        }

        partial void OnSelectedProductChanged(Product? newValue)
        {
            LoadBoughtProducts(newValue?.Id);
            OnPropertyChanged(nameof(IsProductSelected));
        }

        private void LoadBoughtProducts(int? productId)
        {
            BoughtProductsList.Clear();

            if (productId == null)
            {
                // No product selected
                return;
            }

            var productsToLoad = _boughtProductsService.Get(productId);
            foreach (var item in productsToLoad)
            {
                BoughtProductsList.Add(item);
            }
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}