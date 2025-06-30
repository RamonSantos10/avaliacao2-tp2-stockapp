using StockApp.Application.DTOs; 
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using AutoMapper; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task Add(ProductDTO productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            await _productRepository.Create(productEntity);
        }
        public async Task<ProductDTO> GetProductById(int? id)
        {
            var productEntity = await _productRepository.GetById(id);
            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var productsEntity = await _productRepository.GetProducts();
            return _mapper.Map<IEnumerable<ProductDTO>>(productsEntity);
        }

        public async Task Remove(int? id)
        {
            var productEntity = await _productRepository.GetById(id);
            if (productEntity != null)
            {
                await _productRepository.Remove(productEntity);
            }
        }

        public async Task Update(ProductDTO productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            await _productRepository.Update(productEntity);
        }

        public async Task<IEnumerable<ProductDTO>> SearchAsync(string name, decimal? minPrice, decimal? maxPrice)
        {
            var productsEntity = await _productRepository.SearchAsync(name, minPrice, maxPrice);
            return _mapper.Map<IEnumerable<ProductDTO>>(productsEntity);
        }
        public async Task<IEnumerable<ProductDTO>> GetProductsByIdsAsync(List<int> productIds)
        {
            var productsEntity = await _productRepository.GetByIdsAsync(productIds);
            return _mapper.Map<IEnumerable<ProductDTO>>(productsEntity);
        }
    }
}
