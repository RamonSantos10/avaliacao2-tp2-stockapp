using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Controllers;
using StockApp.Infra.Data.Context;
using StockApp.Domain.Entities;
using StockApp.Application.DTOs;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace StockApp.Domain.Test
{
    public class DashboardControllerTests
    {
        [Fact]
        public async Task GetDashboardstockData_ReturnsCorrectData_WhenNoLowStockProducts()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var products = new List<Product>
                {
                    new Product(1, "Produto A", "Descrição A", 100.00m, 15, "imageA.jpg"),
                    new Product(2, "Produto B", "Descrição B", 50.00m, 20, "imageB.jpg"),
                    new Product(3, "Produto C", "Descrição C", 200.00m, 12, "imageC.jpg")
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                var controller = new DashboardController(context);
                var result = await controller.GetDashboardstockData();

                var okResult = Assert.IsType<OkObjectResult>(result);
                var dashboardData = Assert.IsType<DashboardstockDTO>(okResult.Value);

                Assert.Equal(3, dashboardData.TotalProducts);
                Assert.Equal(15 * 100.00m + 20 * 50.00m + 12 * 200.00m, dashboardData.TotalStockValue);
                Assert.Empty(dashboardData.LowStockProducts);
            }
        }

        [Fact]
        public async Task GetDashboardstockData_ReturnsCorrectData_WithLowStockProducts()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var products = new List<Product>
                {
                    new Product(1, "Produto X", "Descrição X", 100.00m, 5, "imageX.jpg"),
                    new Product(2, "Produto Y", "Descrição Y", 50.00m, 20, "imageY.jpg"),
                    new Product(3, "Produto Z", "Descrição Z", 200.00m, 8, "imageZ.jpg")
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                var controller = new DashboardController(context);
                var result = await controller.GetDashboardstockData();

                var okResult = Assert.IsType<OkObjectResult>(result);
                var dashboardData = Assert.IsType<DashboardstockDTO>(okResult.Value);

                Assert.Equal(3, dashboardData.TotalProducts);
                Assert.Equal(5 * 100.00m + 20 * 50.00m + 8 * 200.00m, dashboardData.TotalStockValue);
                Assert.Equal(2, dashboardData.LowStockProducts.Count);

                var lowStockProduct1 = Assert.Single(dashboardData.LowStockProducts, p => p.ProductName == "Produto X");
                Assert.Equal(5, lowStockProduct1.Stock);

                var lowStockProduct2 = Assert.Single(dashboardData.LowStockProducts, p => p.ProductName == "Produto Z");
                Assert.Equal(8, lowStockProduct2.Stock);
            }
        }

        [Fact]
        public async Task GetDashboardstockData_ReturnsEmptyData_WhenNoProducts()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var controller = new DashboardController(context);
                var result = await controller.GetDashboardstockData();

                var okResult = Assert.IsType<OkObjectResult>(result);
                var dashboardData = Assert.IsType<DashboardstockDTO>(okResult.Value);

                Assert.Equal(0, dashboardData.TotalProducts);
                Assert.Equal(0, dashboardData.TotalStockValue);
                Assert.Empty(dashboardData.LowStockProducts);
            }
        }
    }
}