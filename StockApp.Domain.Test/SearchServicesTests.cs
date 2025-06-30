using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using Xunit;                     
using Moq;                       
using StockApp.Application.DTOs; 
using StockApp.Application.Services; 
using AutoMapper;                
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StockApp.Domain.Test
{
    public class SearchServicesTestes
    {
        [Fact]
        public async Task SearchAsync_ShouldReturnFilteredProducts()
        {
            
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();


            var fakeProducts = new List<Product>
            {
                new Product(1, "Banana", "delicious banana", 5, 100, "banana.png"),
                new Product(2, "Apple", "sweet apple", 15, 50, "apple.png") 
            };


            mockRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<decimal?>()))
            .ReturnsAsync((string name, decimal? min, decimal? max) =>
            fakeProducts
            .Where(p =>
                (string.IsNullOrEmpty(name) || p.Name.Contains(name))
                && (!min.HasValue || p.Price >= min)
                && (!max.HasValue || p.Price <= max))
            .ToList());


            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<Product> source) => source.Select(p => new ProductDTO { Name = p.Name, Price = p.Price }));

            var service = new ProductService(mockRepo.Object, mockMapper.Object);


            var result = await service.SearchAsync("Banana", 5m, 20m);

            Assert.Single(result);
            Assert.Equal("Banana", result.First().Name);
        }

        //Retornar lista vazia quando nenhum produto casa
        [Fact]
        public async Task SearchAsync_ShouldReturnEmpty_WhenNoProductMatches()
        {
           
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            var fakeProducts = new List<Product>
            {
                new Product(1, "Banana", "delicious banana", 5, 100, "banana.png"),
                new Product(2, "Banana", "yellow banana", 12, 50, "banana2.png") // << CORRIGIDO
            };


            mockRepo.Setup(r => r.SearchAsync("Laranja", 5m, 20m))
                .ReturnsAsync(new List<Product>()); 

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(Enumerable.Empty<ProductDTO>());

            var service = new ProductService(mockRepo.Object, mockMapper.Object);

            
            var result = await service.SearchAsync("Laranja", 5m, 20m);

           
            Assert.Empty(result);
        }

        //Retornar mais de um produto com nome igual
        [Fact]
        public async Task SearchAsync_ShouldReturnMultipleProducts_WhenMultipleMatch()
        {
            
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            var fakeProducts = new List<Product>
            {
                new Product(1, "Banana", "delicious banana", 5, 100, "banana.png"),
                new Product(2, "Banana", "yellow banana", 15, 50, "banana2.png")
            };


            mockRepo.Setup(r => r.SearchAsync("Banana", 5m, 20m))
                .ReturnsAsync(fakeProducts);

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<Product> source) =>
                    source.Select(p => new ProductDTO { Name = p.Name, Price = p.Price }));

            var service = new ProductService(mockRepo.Object, mockMapper.Object);

            
            var result = await service.SearchAsync("Banana", 5m, 20m);

            
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal("Banana", r.Name));
        }


        //Nome null deve retornar todos os produtos dentro da faixa de preço
        [Fact]
        public async Task SearchAsync_ShouldReturnAllInRange_WhenNameIsNull()
        {
           
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            var fakeProducts = new List<Product>
        {
            new Product(1, "Banana", "delicious banana", 5, 100, "banana.png"),
            new Product(2, "Banana", "ripe banana", 999, 50, "banana2.png") 
         };

            mockRepo.Setup(r => r.SearchAsync(null, 5m, 20m))
                .ReturnsAsync(fakeProducts);

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<Product> source) =>
                    source.Select(p => new ProductDTO { Name = p.Name, Price = p.Price }));

            var service = new ProductService(mockRepo.Object, mockMapper.Object);

            
            var result = await service.SearchAsync(null, 5m, 20m);

            Assert.Equal(2, result.Count());
        }


        // minPrice e maxPrice nulos devem retornar tudo com o nome filtrado
        [Fact]
        public async Task SearchAsync_ShouldIgnorePriceFilter_WhenPricesAreNull()
        {
            
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            var fakeProducts = new List<Product>
            {
                new Product(1, "Banana", "delicious banana", 5, 100, "banana.png"),
                new Product(2, "Banana", "ripe banana", 999, 50, "banana2.png")

            };

            mockRepo.Setup(r => r.SearchAsync("Banana", null, null))
                .ReturnsAsync(fakeProducts);

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<Product> source) =>
                    source.Select(p => new ProductDTO { Name = p.Name, Price = p.Price }));

            var service = new ProductService(mockRepo.Object, mockMapper.Object);

            
            var result = await service.SearchAsync("Banana", null, null);

            Assert.Equal(2, result.Count());
        }

    }
}