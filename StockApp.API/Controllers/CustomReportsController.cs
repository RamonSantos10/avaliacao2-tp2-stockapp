using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomReportsController : ControllerBase
    {
        private readonly ICustomReportService _customReportService;

        public CustomReportsController(ICustomReportService customReportService)
        {
            _customReportService = customReportService;
        }

        /// <summary>
        /// Gera um relatório personalizado baseado nos parâmetros fornecidos
        /// </summary>
        /// <param name="parameters">Parâmetros do relatório</param>
        /// <returns>Relatório personalizado gerado</returns>
        [HttpPost("generate")]
        public async Task<ActionResult<CustomReportDto>> GenerateReport([FromBody] ReportParametersDto parameters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var report = await _customReportService.GenerateReportAsync(parameters);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Gera um relatório específico de vendas
        /// </summary>
        /// <param name="parameters">Parâmetros do relatório de vendas</param>
        /// <returns>Relatório de vendas</returns>
        [HttpPost("sales")]
        public async Task<ActionResult<CustomReportDto>> GenerateSalesReport([FromBody] ReportParametersDto parameters)
        {
            try
            {
                var report = await _customReportService.GenerateSalesReportAsync(parameters);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar relatório de vendas", details = ex.Message });
            }
        }

        /// <summary>
        /// Gera um relatório específico de inventário
        /// </summary>
        /// <param name="parameters">Parâmetros do relatório de inventário</param>
        /// <returns>Relatório de inventário</returns>
        [HttpPost("inventory")]
        public async Task<ActionResult<CustomReportDto>> GenerateInventoryReport([FromBody] ReportParametersDto parameters)
        {
            try
            {
                var report = await _customReportService.GenerateInventoryReportAsync(parameters);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar relatório de inventário", details = ex.Message });
            }
        }

        /// <summary>
        /// Gera um relatório de performance de produtos
        /// </summary>
        /// <param name="parameters">Parâmetros do relatório de performance</param>
        /// <returns>Relatório de performance</returns>
        [HttpPost("performance")]
        public async Task<ActionResult<CustomReportDto>> GenerateProductPerformanceReport([FromBody] ReportParametersDto parameters)
        {
            try
            {
                var report = await _customReportService.GenerateProductPerformanceReportAsync(parameters);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar relatório de performance", details = ex.Message });
            }
        }

        /// <summary>
        /// Gera um relatório por categoria
        /// </summary>
        /// <param name="parameters">Parâmetros do relatório por categoria</param>
        /// <returns>Relatório por categoria</returns>
        [HttpPost("category")]
        public async Task<ActionResult<CustomReportDto>> GenerateCategoryReport([FromBody] ReportParametersDto parameters)
        {
            try
            {
                var report = await _customReportService.GenerateCategoryReportAsync(parameters);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar relatório por categoria", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o histórico de relatórios gerados
        /// </summary>
        /// <returns>Lista de relatórios gerados anteriormente</returns>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<CustomReportDto>>> GetReportHistory()
        {
            try
            {
                var history = await _customReportService.GetReportHistoryAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter histórico de relatórios", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os tipos de relatórios disponíveis
        /// </summary>
        /// <returns>Lista de tipos de relatórios disponíveis</returns>
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableReportTypes()
        {
            try
            {
                var types = await _customReportService.GetAvailableReportTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter tipos de relatórios", details = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para testar a funcionalidade básica de relatórios
        /// </summary>
        /// <returns>Relatório de teste</returns>
        [HttpGet("test")]
        public async Task<ActionResult<CustomReportDto>> TestReport()
        {
            try
            {
                var testParameters = new ReportParametersDto
                {
                    ReportType = "test",
                    IncludeDetails = true
                };

                var report = new CustomReportDto
                {
                    Title = "Relatório de Teste",
                    ReportType = "Test",
                    Data = new List<ReportDataDto>
                    {
                        new ReportDataDto { Key = "TotalVendas", Value = "10000", Description = "Total de vendas simuladas" },
                        new ReportDataDto { Key = "TotalPedidos", Value = "200", Description = "Total de pedidos simulados" }
                    },
                    Summary = new ReportSummaryDto
                    {
                        TotalRecords = 2,
                        TotalValue = 10000,
                        AdditionalMetrics = new Dictionary<string, object>
                        {
                            { "Status", "Teste executado com sucesso" }
                        }
                    }
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro no teste de relatórios", details = ex.Message });
            }
        }
    }
}