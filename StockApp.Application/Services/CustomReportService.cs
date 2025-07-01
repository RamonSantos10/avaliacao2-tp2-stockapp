using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class CustomReportService : ICustomReportService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private static readonly List<CustomReportDto> _reportHistory = new List<CustomReportDto>();

        public CustomReportService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CustomReportDto> GenerateReportAsync(ReportParametersDto parameters)
        {
            var report = parameters.ReportType?.ToLower() switch
            {
                "sales" => await GenerateSalesReportAsync(parameters),
                "inventory" => await GenerateInventoryReportAsync(parameters),
                "performance" => await GenerateProductPerformanceReportAsync(parameters),
                "category" => await GenerateCategoryReportAsync(parameters),
                _ => await GenerateDefaultReportAsync(parameters)
            };

            _reportHistory.Add(report);
            return report;
        }

        public async Task<CustomReportDto> GenerateSalesReportAsync(ReportParametersDto parameters)
        {
            var products = await _productRepository.GetProducts();
            var filteredProducts = FilterProductsByParameters(products, parameters);

            var reportData = new List<ReportDataDto>();
            decimal totalSales = 0;
            int totalOrders = 0;

            foreach (var product in filteredProducts)
            {
                var salesValue = product.Price * (product.Stock > 0 ? Math.Max(1, 100 - product.Stock) : 0);
                totalSales += salesValue;
                totalOrders += product.Stock > 0 ? Math.Max(1, 100 - product.Stock) : 0;

                reportData.Add(new ReportDataDto
                {
                    Key = product.Name,
                    Value = salesValue.ToString("C2"),
                    Description = $"Vendas do produto {product.Name}",
                    Category = "Vendas",
                    Date = DateTime.Now.AddDays(-new Random().Next(1, 30))
                });
            }

            return new CustomReportDto
            {
                Title = "Relatório de Vendas",
                ReportType = "Sales",
                Data = reportData,
                Summary = new ReportSummaryDto
                {
                    TotalRecords = reportData.Count,
                    TotalValue = totalSales,
                    AdditionalMetrics = new Dictionary<string, object>
                    {
                        { "TotalOrders", totalOrders },
                        { "AverageOrderValue", totalOrders > 0 ? totalSales / totalOrders : 0 }
                    }
                }
            };
        }

        public async Task<CustomReportDto> GenerateInventoryReportAsync(ReportParametersDto parameters)
        {
            var products = await _productRepository.GetProducts();
            var filteredProducts = FilterProductsByParameters(products, parameters);

            var reportData = new List<ReportDataDto>();
            decimal totalInventoryValue = 0;
            int lowStockCount = 0;

            foreach (var product in filteredProducts)
            {
                var inventoryValue = product.Price * product.Stock;
                totalInventoryValue += inventoryValue;
                
                if (product.Stock < 10)
                    lowStockCount++;

                reportData.Add(new ReportDataDto
                {
                    Key = product.Name,
                    Value = product.Stock.ToString(),
                    Description = $"Estoque atual: {product.Stock} unidades (Valor: {inventoryValue:C2})",
                    Category = product.Stock < 10 ? "Estoque Baixo" : "Estoque Normal",
                    Date = DateTime.Now
                });
            }

            return new CustomReportDto
            {
                Title = "Relatório de Inventário",
                ReportType = "Inventory",
                Data = reportData,
                Summary = new ReportSummaryDto
                {
                    TotalRecords = reportData.Count,
                    TotalValue = totalInventoryValue,
                    AdditionalMetrics = new Dictionary<string, object>
                    {
                        { "LowStockItems", lowStockCount },
                        { "AverageStockValue", reportData.Count > 0 ? totalInventoryValue / reportData.Count : 0 }
                    }
                }
            };
        }

        public async Task<CustomReportDto> GenerateProductPerformanceReportAsync(ReportParametersDto parameters)
        {
            var products = await _productRepository.GetProducts();
            var filteredProducts = FilterProductsByParameters(products, parameters);

            var reportData = new List<ReportDataDto>();
            var performanceScores = new List<decimal>();

            foreach (var product in filteredProducts)
            {
                var performanceScore = CalculatePerformanceScore(product);
                performanceScores.Add(performanceScore);

                reportData.Add(new ReportDataDto
                {
                    Key = product.Name,
                    Value = performanceScore.ToString("F2"),
                    Description = $"Score de performance baseado em preço, estoque e categoria",
                    Category = GetPerformanceCategory(performanceScore),
                    Date = DateTime.Now
                });
            }

            return new CustomReportDto
            {
                Title = "Relatório de Performance de Produtos",
                ReportType = "Performance",
                Data = reportData,
                Summary = new ReportSummaryDto
                {
                    TotalRecords = reportData.Count,
                    TotalValue = performanceScores.Sum(),
                    AdditionalMetrics = new Dictionary<string, object>
                    {
                        { "AverageScore", performanceScores.Count > 0 ? performanceScores.Average() : 0 },
                        { "TopPerformers", reportData.Count(r => r.Category == "Alto Desempenho") }
                    }
                }
            };
        }

        public async Task<CustomReportDto> GenerateCategoryReportAsync(ReportParametersDto parameters)
        {
            var products = await _productRepository.GetProducts();
            var categories = await _categoryRepository.GetCategories();

            var reportData = new List<ReportDataDto>();
            decimal totalCategoryValue = 0;

            var categoryGroups = products.GroupBy(p => p.CategoryId);

            foreach (var group in categoryGroups)
            {
                var category = categories.FirstOrDefault(c => c.Id == group.Key);
                var categoryProducts = group.ToList();
                var categoryValue = categoryProducts.Sum(p => p.Price * p.Stock);
                totalCategoryValue += categoryValue;

                reportData.Add(new ReportDataDto
                {
                    Key = category?.Name ?? "Categoria Desconhecida",
                    Value = categoryProducts.Count.ToString(),
                    Description = $"{categoryProducts.Count} produtos, Valor total: {categoryValue:C2}",
                    Category = "Categoria",
                    Date = DateTime.Now
                });
            }

            return new CustomReportDto
            {
                Title = "Relatório por Categoria",
                ReportType = "Category",
                Data = reportData,
                Summary = new ReportSummaryDto
                {
                    TotalRecords = reportData.Count,
                    TotalValue = totalCategoryValue,
                    AdditionalMetrics = new Dictionary<string, object>
                    {
                        { "TotalCategories", reportData.Count },
                        { "AverageCategoryValue", reportData.Count > 0 ? totalCategoryValue / reportData.Count : 0 }
                    }
                }
            };
        }

        public async Task<IEnumerable<CustomReportDto>> GetReportHistoryAsync()
        {
            return await Task.FromResult(_reportHistory.OrderByDescending(r => r.GeneratedAt));
        }

        public async Task<IEnumerable<string>> GetAvailableReportTypesAsync()
        {
            var reportTypes = new List<string>
            {
                "Sales",
                "Inventory",
                "Performance",
                "Category"
            };

            return await Task.FromResult(reportTypes);
        }

        private async Task<CustomReportDto> GenerateDefaultReportAsync(ReportParametersDto parameters)
        {
            var products = await _productRepository.GetProducts();
            var totalProducts = products.Count();
            var totalValue = products.Sum(p => p.Price * p.Stock);

            return new CustomReportDto
            {
                Title = "Relatório Personalizado",
                ReportType = "Default",
                Data = new List<ReportDataDto>
                {
                    new ReportDataDto { Key = "TotalProdutos", Value = totalProducts.ToString(), Description = "Total de produtos cadastrados" },
                    new ReportDataDto { Key = "ValorTotalEstoque", Value = totalValue.ToString("C2"), Description = "Valor total do estoque" }
                },
                Summary = new ReportSummaryDto
                {
                    TotalRecords = 2,
                    TotalValue = totalValue
                }
            };
        }

        private IEnumerable<Domain.Entities.Product> FilterProductsByParameters(IEnumerable<Domain.Entities.Product> products, ReportParametersDto parameters)
        {
            var filtered = products.AsQueryable();

            if (parameters.CategoryId.HasValue)
                filtered = filtered.Where(p => p.CategoryId == parameters.CategoryId.Value);

            if (parameters.ProductId.HasValue)
                filtered = filtered.Where(p => p.Id == parameters.ProductId.Value);

            return filtered.ToList();
        }

        private decimal CalculatePerformanceScore(Domain.Entities.Product product)
        {

            var priceScore = Math.Min(product.Price / 100, 10);
            var stockScore = Math.Min(product.Stock / 10, 10);
            var categoryScore = 5;

            return (priceScore + stockScore + categoryScore) / 3;
        }

        private string GetPerformanceCategory(decimal score)
        {
            return score switch
            {
                >= 8 => "Alto Desempenho",
                >= 6 => "Médio Desempenho",
                >= 4 => "Baixo Desempenho",
                _ => "Desempenho Crítico"
            };
        }
    }
}