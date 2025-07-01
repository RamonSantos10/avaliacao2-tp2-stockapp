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
    public class EmployeePerformanceController : ControllerBase
    {
        private readonly IEmployeePerformanceEvaluationService _evaluationService;

        public EmployeePerformanceController(IEmployeePerformanceEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        /// <summary>
        /// Avalia o desempenho de um funcionário específico
        /// </summary>
        /// <param name="employeeId">ID do funcionário</param>
        /// <returns>Avaliação de desempenho do funcionário</returns>
        [HttpGet("evaluate/{employeeId}")]
        public async Task<ActionResult<EmployeeEvaluationDTO>> EvaluatePerformance(int employeeId)
        {
            try
            {
                var evaluation = await _evaluationService.EvaluatePerformanceAsync(employeeId);
                return Ok(evaluation);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova avaliação de funcionário
        /// </summary>
        /// <param name="createEvaluationDto">Dados da avaliação</param>
        /// <returns>Avaliação criada</returns>
        [HttpPost("evaluations")]
        public async Task<ActionResult<EmployeeEvaluationDTO>> CreateEvaluation([FromBody] CreateEmployeeEvaluationDTO createEvaluationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var evaluation = await _evaluationService.CreateEvaluationAsync(createEvaluationDto);
                return CreatedAtAction(nameof(GetEvaluationById), new { id = evaluation.Id }, evaluation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma avaliação existente
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        /// <param name="updateEvaluationDto">Dados atualizados da avaliação</param>
        /// <returns>Avaliação atualizada</returns>
        [HttpPut("evaluations/{id}")]
        public async Task<ActionResult<EmployeeEvaluationDTO>> UpdateEvaluation(int id, [FromBody] CreateEmployeeEvaluationDTO updateEvaluationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var evaluation = await _evaluationService.UpdateEvaluationAsync(id, updateEvaluationDto);
                return Ok(evaluation);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma avaliação por ID
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        /// <returns>Avaliação encontrada</returns>
        [HttpGet("evaluations/{id}")]
        public async Task<ActionResult<EmployeeEvaluationDTO>> GetEvaluationById(int id)
        {
            try
            {
                var evaluation = await _evaluationService.GetEvaluationByIdAsync(id);
                return Ok(evaluation);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todas as avaliações
        /// </summary>
        /// <returns>Lista de todas as avaliações</returns>
        [HttpGet("evaluations")]
        public async Task<ActionResult<IEnumerable<EmployeeEvaluationDTO>>> GetAllEvaluations()
        {
            try
            {
                var evaluations = await _evaluationService.GetAllEvaluationsAsync();
                return Ok(evaluations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém avaliações de um funcionário específico
        /// </summary>
        /// <param name="employeeId">ID do funcionário</param>
        /// <returns>Lista de avaliações do funcionário</returns>
        [HttpGet("evaluations/employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeEvaluationDTO>>> GetEvaluationsByEmployeeId(int employeeId)
        {
            try
            {
                var evaluations = await _evaluationService.GetEvaluationsByEmployeeIdAsync(employeeId);
                return Ok(evaluations);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém avaliações por período
        /// </summary>
        /// <param name="period">Período da avaliação (ex: Q1 2025)</param>
        /// <returns>Lista de avaliações do período</returns>
        [HttpGet("evaluations/period/{period}")]
        public async Task<ActionResult<IEnumerable<EmployeeEvaluationDTO>>> GetEvaluationsByPeriod(string period)
        {
            try
            {
                var evaluations = await _evaluationService.GetEvaluationsByPeriodAsync(period);
                return Ok(evaluations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a última avaliação de um funcionário
        /// </summary>
        /// <param name="employeeId">ID do funcionário</param>
        /// <returns>Última avaliação do funcionário</returns>
        [HttpGet("evaluations/employee/{employeeId}/latest")]
        public async Task<ActionResult<EmployeeEvaluationDTO>> GetLatestEvaluationByEmployeeId(int employeeId)
        {
            try
            {
                var evaluation = await _evaluationService.GetLatestEvaluationByEmployeeIdAsync(employeeId);
                if (evaluation == null)
                {
                    return NotFound(new { message = $"No evaluations found for employee {employeeId}" });
                }
                return Ok(evaluation);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém avaliações por faixa de pontuação
        /// </summary>
        /// <param name="minScore">Pontuação mínima</param>
        /// <param name="maxScore">Pontuação máxima</param>
        /// <returns>Lista de avaliações na faixa especificada</returns>
        [HttpGet("evaluations/score-range")]
        public async Task<ActionResult<IEnumerable<EmployeeEvaluationDTO>>> GetEvaluationsByScoreRange([FromQuery] int minScore = 0, [FromQuery] int maxScore = 100)
        {
            try
            {
                if (minScore < 0 || maxScore > 100 || minScore > maxScore)
                {
                    return BadRequest(new { message = "Invalid score range. Scores must be between 0-100 and minScore <= maxScore" });
                }

                var evaluations = await _evaluationService.GetEvaluationsByScoreRangeAsync(minScore, maxScore);
                return Ok(evaluations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Exclui uma avaliação
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("evaluations/{id}")]
        public async Task<ActionResult> DeleteEvaluation(int id)
        {
            try
            {
                await _evaluationService.DeleteEvaluationAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém informações de um funcionário
        /// </summary>
        /// <param name="employeeId">ID do funcionário</param>
        /// <returns>Informações do funcionário</returns>
        [HttpGet("employees/{employeeId}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeById(int employeeId)
        {
            try
            {
                var employee = await _evaluationService.GetEmployeeByIdAsync(employeeId);
                return Ok(employee);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os funcionários
        /// </summary>
        /// <returns>Lista de todos os funcionários</returns>
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAllEmployees()
        {
            try
            {
                var employees = await _evaluationService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }
    }
}