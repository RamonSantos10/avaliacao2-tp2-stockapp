using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesPredictionController : ControllerBase
    {
        private readonly ISalesPredictionService _predictionService;

        public SalesPredictionController(ISalesPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost("predict")]
        public async Task<ActionResult<SalesPredictionDTO>> PredictSales([FromBody] SalesPredictionInputDTO input)
        {
            try
            {
                var prediction = await _predictionService.PredictSalesAsync(input);
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao gerar previsão de vendas: {ex.Message}");
            }
        }

        [HttpGet("history/{productId}")]
        public async Task<ActionResult> GetPredictionHistory(int productId)
        {
            try
            {
                var history = await _predictionService.GetHistoricalPredictionsAsync(productId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao recuperar histórico de previsões: {ex.Message}");
            }
        }

        [HttpGet("accuracy/{productId}")]
        public async Task<ActionResult<double>> GetPredictionAccuracy(int productId)
        {
            try
            {
                var accuracy = await _predictionService.GetPredictionAccuracyAsync(productId);
                return Ok(accuracy);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao calcular precisão das previsões: {ex.Message}");
            }
        }

        [HttpPost("update-model")]
        public async Task<ActionResult> UpdateModel()
        {
            try
            {
                await _predictionService.UpdateModelAsync();
                return Ok("Modelo atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar modelo: {ex.Message}");
            }
        }

        [HttpGet("insights/{productId}")]
        public async Task<ActionResult> GetSalesInsights(int productId)
        {
            try
            {
                var insights = await _predictionService.GetSalesInsightsAsync(productId);
                return Ok(insights);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter insights de vendas: {ex.Message}");
            }
        }
    }
}