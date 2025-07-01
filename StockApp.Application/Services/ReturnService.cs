using AutoMapper;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;

public class ReturnService : IReturnService
{
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReturnService(IProductRepository productRepository, ApplicationDbContext context, IMapper mapper)
    {
        _productRepository = productRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> ProcessReturnAsync(ReturnProductDTO returnProductDto)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(returnProductDto.ProductId);
            if (product == null)
                return false;

            var returnRecord = new Return
            {
                OrderId = returnProductDto.OrderId,
                ProductId = returnProductDto.ProductId,
                Quantity = returnProductDto.Quantity,
                ReturnReason = returnProductDto.ReturnReason,
                ReturnDate = DateTime.Now,
                Status = "Approved"
            };

            product.Stock += returnProductDto.Quantity;

            _context.Returns.Add(returnRecord);
            await _productRepository.UpdateAsync(product);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            throw;
        }
    }
}