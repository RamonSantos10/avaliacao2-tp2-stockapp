using AutoMapper;
using Moq;
using StockApp.Application.DTOs;
using StockApp.Application.Services;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDTO>();
        });
        _mapper = config.CreateMapper();

        _service = new ProductService(_mockRepo.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedProducts()
    {
        var products = new List<Product>
    {
        new Product("Produto 1", "Descrição 1", 10m, 5, "imagem1.jpg"),
        new Product("Produto 2", "Descrição 2", 20m, 3, "imagem2.jpg"),
        new Product("Produto 3", "Descrição 3", 30m, 2, "imagem3.jpg")
    };

        _mockRepo.Setup(r => r.GetAllAsync(1, 2))
            .ReturnsAsync(products.Take(2));

        var result = await _service.GetAllAsync(1, 2);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Produto 1", result.First().Name);
    }

}
