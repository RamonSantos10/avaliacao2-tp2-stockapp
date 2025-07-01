using Microsoft.ML;
using Microsoft.ML.Data;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class SalesPredictionService : ISalesPredictionService
    {
        private readonly IProductRepository _productRepository;
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public SalesPredictionService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _mlContext = new MLContext(seed: 1);
        }

        public async Task<SalesPredictionDTO> PredictSalesAsync(SalesPredictionInputDTO input)
        {
            // Validações de entrada
            if (input.ProductId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than zero.", nameof(input.ProductId));
            }

            if (input.TargetDate <= DateTime.Now)
            {
                throw new ArgumentException("Target date must be in the future.", nameof(input.TargetDate));
            }

            var historicalData = await GetHistoricalSalesData(input.ProductId, input.HistoricalMonths);
            
            if (_model == null)
            {
                await TrainModel(historicalData);
            }

            var prediction = MakePrediction(input);
            
            // Garantir que os valores estão no intervalo esperado
            var confidence = prediction.Score;
            if (float.IsNaN(confidence) || float.IsInfinity(confidence))
                confidence = 0.5f; // Valor padrão
            else
                confidence = Math.Max(0, Math.Min(1, Math.Abs(confidence)));

            return new SalesPredictionDTO
            {
                ProductId = input.ProductId,
                PredictionDate = input.TargetDate,
                PredictedQuantity = Math.Max(0, prediction.Prediction),
                Confidence = confidence,
                ModelVersion = "1.0",
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<SalesPredictionDTO>> GetHistoricalPredictionsAsync(int productId)
        {
            // Implementar recuperação de previsões históricas do banco de dados
            return new List<SalesPredictionDTO>();
        }

        public async Task<double> GetPredictionAccuracyAsync(int productId)
        {
            // Implementar cálculo de precisão comparando previsões anteriores com vendas reais
            return 0.85; // Exemplo de retorno
        }

        public async Task UpdateModelAsync()
        {
            var allProducts = await _productRepository.GetProducts();
            var historicalData = await GetHistoricalSalesData(0, 24); // Últimos 24 meses de todos os produtos
            await TrainModel(historicalData);
        }

        public async Task<Dictionary<string, double>> GetSalesInsightsAsync(int productId)
        {
            return new Dictionary<string, double>
            {
                { "TrendGrowth", 0.15 },
                { "SeasonalityIndex", 1.2 },
                { "PredictionConfidence", 0.85 }
            };
        }

        private async Task<IEnumerable<SalesDataPoint>> GetHistoricalSalesData(int productId, int months)
        {
            // Para testes, retornar dados de exemplo
            // Em produção, implementar lógica para buscar dados históricos de vendas do banco
            var sampleData = new List<SalesDataPoint>();
            
            for (int i = 1; i <= Math.Max(months, 12); i++)
            {
                var date = DateTime.Now.AddMonths(-i);
                sampleData.Add(new SalesDataPoint
                {
                    ProductId = productId > 0 ? productId : 1,
                    Month = date.Month,
                    Year = date.Year,
                    Sales = (float)(100 + (i * 10) + new Random(i).Next(0, 50))
                });
            }
            
            return sampleData;
        }

        private async Task TrainModel(IEnumerable<SalesDataPoint> historicalData)
        {
            var trainingData = _mlContext.Data.LoadFromEnumerable(historicalData);

            var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Sales")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "ProductIdEncoded", inputColumnName: "ProductId"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "MonthEncoded", inputColumnName: "Month"))
                .Append(_mlContext.Transforms.Conversion.ConvertType("YearFloat", "Year", DataKind.Single))
                .Append(_mlContext.Transforms.Concatenate("Features", "ProductIdEncoded", "MonthEncoded", "YearFloat"))
                .Append(_mlContext.Regression.Trainers.FastForest());

            _model = pipeline.Fit(trainingData);
        }

        private PredictionResult MakePrediction(SalesPredictionInputDTO input)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SalesDataPoint, PredictionResult>(_model);

            var dataPoint = new SalesDataPoint
            {
                ProductId = input.ProductId,
                Month = input.TargetDate.Month,
                Year = input.TargetDate.Year,
                Sales = 0 // Não utilizado para previsão
            };

            return predictionEngine.Predict(dataPoint);
        }
    }

    public class SalesDataPoint
    {
        [LoadColumn(0)]
        public int ProductId { get; set; }

        [LoadColumn(1)]
        public int Month { get; set; }

        [LoadColumn(2)]
        public int Year { get; set; }

        [LoadColumn(3)]
        public float Sales { get; set; }
    }

    public class PredictionResult
    {
        [ColumnName("Score")]
        public float Score { get; set; }

        [ColumnName("Prediction")]
        public float Prediction { get; set; }
    }
}