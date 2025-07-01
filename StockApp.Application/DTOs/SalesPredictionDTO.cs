using System;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class SalesPredictionDTO
    {
        public int ProductId { get; set; }
        
        [Required]
        public DateTime PredictionDate { get; set; }
        
        public double PredictedQuantity { get; set; }
        
        public double Confidence { get; set; }
        
        public string ModelVersion { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }

    public class SalesPredictionInputDTO
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public DateTime TargetDate { get; set; }
        
        public int HistoricalMonths { get; set; } = 12;
    }
}