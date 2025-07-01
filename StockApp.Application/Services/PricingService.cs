using StockApp.Application.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using StockApp.Application.DTOs;

namespace StockApp.Application.Services
{
    public class PricingService : IPricingService
    {
        private readonly HttpClient _httpClient;

        public PricingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductPricingDTO> GetProductDetailsAsync(string productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products/{productId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var productData = JsonConvert.DeserializeObject<DummyJsonProduct>(content);

                if (productData == null)
                {
                    return GetDefaultProductDetails(productId);
                }

                return new ProductPricingDTO
                {
                    Id = productData.Id.ToString(),
                    Name = productData.Title,
                    Description = productData.Description,
                    Price = productData.Price,
                    Stock = productData.Stock,
                    Category = productData.Category,
                    Brand = productData.Brand
                };
            }
            catch (HttpRequestException)
            {
                return GetDefaultProductDetails(productId);
            }
        }

        private ProductPricingDTO GetDefaultProductDetails(string productId)
        {
            return new ProductPricingDTO
            {
                Id = productId,
                Name = "Produto Não Encontrado",
                Description = "Descrição não disponível",
                Price = 99.99m,
                Stock = 0,
                Category = "Geral",
                Brand = "N/A"
            };
        }
    }

    public class DummyJsonProduct
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }
    }
}