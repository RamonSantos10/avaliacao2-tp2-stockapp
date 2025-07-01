namespace StockApp.Application.Interfaces
{
    public interface IReturnService
    {
        Task<bool> ProcessReturnAsync(ReturnProductDTO returnProductDto);
    }
}
