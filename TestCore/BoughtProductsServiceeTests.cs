using FluentAssertions;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Moq;

namespace TestCore
{
    [TestFixture]
    public class BoughtProductsServiceTests
    {
        private Mock<IGroceryListItemsRepository> _groceryListItemsRepositoryMock;
        private Mock<IGroceryListRepository> _groceryListRepositoryMock;
        private Mock<IClientRepository> _clientRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private BoughtProductsService _sut;

        [SetUp]
        public void Setup()
        {
            _groceryListItemsRepositoryMock = new Mock<IGroceryListItemsRepository>();
            _groceryListRepositoryMock = new Mock<IGroceryListRepository>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();

            _sut = new BoughtProductsService(
                _groceryListItemsRepositoryMock.Object,
                _groceryListRepositoryMock.Object,
                _clientRepositoryMock.Object,
                _productRepositoryMock.Object);
        }

        [Test]
        public void Get_WithValidProductId_ReturnsMatchingBoughtProducts()
        {
            // Arrange
            var productId = 1;
            var client = new Client(1, "John Doe", "Johnny.email.com", "balls123", Role.Admin);
            var product = new Product(productId, "Apple", 10);
            var groceryList = new GroceryList(1, "My List", new DateOnly(2024, 1, 1), "green", client.Id);
            var groceryListItems = new List<GroceryListItem>
            {
                new GroceryListItem(1, groceryList.Id, productId, 5),
                new GroceryListItem(2, 2, 2, 3)
            };

            _groceryListItemsRepositoryMock.Setup(r => r.GetAll()).Returns(groceryListItems);
            _groceryListRepositoryMock.Setup(r => r.Get(groceryList.Id)).Returns(groceryList);
            _clientRepositoryMock.Setup(r => r.Get(client.Id)).Returns(client);
            _productRepositoryMock.Setup(r => r.Get(productId)).Returns(product);

            // Act
            var result = _sut.Get(productId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Client.Should().Be(client);
            result.First().Product.Should().Be(product);
            result.First().GroceryList.Should().Be(groceryList);
        }
    }
}
