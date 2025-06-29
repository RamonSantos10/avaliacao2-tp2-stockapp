namespace StockApp.Application.Interfaces
{
    public interface IJustInTimeInventoryService
    {
        Task OptimizeInventoryAsync();
    }
}