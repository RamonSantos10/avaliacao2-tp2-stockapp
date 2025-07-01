using StockApp.Application.Interfaces;

namespace StockApp.Application.Services
{
    public class TaxService : ITaxService
    {
        public decimal CalculateTax(decimal amount)
        {
            return amount * 0.15M;
        }
    }
}
