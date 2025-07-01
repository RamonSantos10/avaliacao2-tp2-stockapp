using StockApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface ISalesPredictionService
    {
        Task<SalesPredictionDTO> PredictSalesAsync(SalesPredictionInputDTO input);
        
        Task<IEnumerable<SalesPredictionDTO>> GetHistoricalPredictionsAsync(int productId);
        
        Task<double> GetPredictionAccuracyAsync(int productId);
        
        Task UpdateModelAsync();
        
        Task<Dictionary<string, double>> GetSalesInsightsAsync(int productId);
    }
}