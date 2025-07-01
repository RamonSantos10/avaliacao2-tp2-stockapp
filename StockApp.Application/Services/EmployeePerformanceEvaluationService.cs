using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class EmployeePerformanceEvaluationService : IEmployeePerformanceEvaluationService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeEvaluationRepository _evaluationRepository;
        private readonly IMapper _mapper;

        public EmployeePerformanceEvaluationService(
            IEmployeeRepository employeeRepository,
            IEmployeeEvaluationRepository evaluationRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _evaluationRepository = evaluationRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeEvaluationDTO> EvaluatePerformanceAsync(int employeeId)
        {
            // Verifica se o funcionário existe
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {employeeId} not found.");
            }

            // Busca a última avaliação do funcionário
            var latestEvaluation = await _evaluationRepository.GetLatestByEmployeeIdAsync(employeeId);
            
            if (latestEvaluation == null)
            {
                // Se não há avaliação anterior, retorna uma avaliação padrão
                return new EmployeeEvaluationDTO
                {
                    EmployeeId = employeeId,
                    EmployeeName = employee.Name,
                    EvaluationScore = 85, // Score padrão conforme especificação
                    Feedback = "Excelente desempenho", // Feedback padrão conforme especificação
                    EvaluationDate = DateTime.UtcNow,
                    EvaluationPeriod = $"Q{(DateTime.UtcNow.Month - 1) / 3 + 1} {DateTime.UtcNow.Year}",
                    EvaluatedBy = "Sistema",
                    CreatedAt = DateTime.UtcNow
                };
            }

            // Mapeia a última avaliação para DTO
            var evaluationDto = _mapper.Map<EmployeeEvaluationDTO>(latestEvaluation);
            evaluationDto.EmployeeName = employee.Name;
            
            return evaluationDto;
        }

        public async Task<EmployeeEvaluationDTO> CreateEvaluationAsync(CreateEmployeeEvaluationDTO createEvaluationDto)
        {
            // Verifica se o funcionário existe
            var employee = await _employeeRepository.GetByIdAsync(createEvaluationDto.EmployeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {createEvaluationDto.EmployeeId} not found.");
            }

            var evaluation = _mapper.Map<EmployeeEvaluation>(createEvaluationDto);
            evaluation.CreatedAt = DateTime.UtcNow;

            var createdEvaluation = await _evaluationRepository.CreateAsync(evaluation);
            var evaluationDto = _mapper.Map<EmployeeEvaluationDTO>(createdEvaluation);
            evaluationDto.EmployeeName = employee.Name;

            return evaluationDto;
        }

        public async Task<EmployeeEvaluationDTO> UpdateEvaluationAsync(int id, CreateEmployeeEvaluationDTO updateEvaluationDto)
        {
            var existingEvaluation = await _evaluationRepository.GetByIdAsync(id);
            if (existingEvaluation == null)
            {
                throw new ArgumentException($"Evaluation with ID {id} not found.");
            }

            // Verifica se o funcionário existe
            var employee = await _employeeRepository.GetByIdAsync(updateEvaluationDto.EmployeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {updateEvaluationDto.EmployeeId} not found.");
            }

            _mapper.Map(updateEvaluationDto, existingEvaluation);
            existingEvaluation.UpdatedAt = DateTime.UtcNow;

            var updatedEvaluation = await _evaluationRepository.UpdateAsync(existingEvaluation);
            var evaluationDto = _mapper.Map<EmployeeEvaluationDTO>(updatedEvaluation);
            evaluationDto.EmployeeName = employee.Name;

            return evaluationDto;
        }

        public async Task<EmployeeEvaluationDTO> GetEvaluationByIdAsync(int id)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(id);
            if (evaluation == null)
            {
                throw new ArgumentException($"Evaluation with ID {id} not found.");
            }

            var evaluationDto = _mapper.Map<EmployeeEvaluationDTO>(evaluation);
            evaluationDto.EmployeeName = evaluation.Employee?.Name;

            return evaluationDto;
        }

        public async Task<IEnumerable<EmployeeEvaluationDTO>> GetAllEvaluationsAsync()
        {
            var evaluations = await _evaluationRepository.GetAllAsync();
            var evaluationDtos = _mapper.Map<IEnumerable<EmployeeEvaluationDTO>>(evaluations);

            // Adiciona o nome do funcionário para cada avaliação
            foreach (var dto in evaluationDtos)
            {
                var evaluation = evaluations.FirstOrDefault(e => e.Id == dto.Id);
                dto.EmployeeName = evaluation?.Employee?.Name;
            }

            return evaluationDtos;
        }

        public async Task<IEnumerable<EmployeeEvaluationDTO>> GetEvaluationsByEmployeeIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {employeeId} not found.");
            }

            var evaluations = await _evaluationRepository.GetByEmployeeIdAsync(employeeId);
            var evaluationDtos = _mapper.Map<IEnumerable<EmployeeEvaluationDTO>>(evaluations);

            foreach (var dto in evaluationDtos)
            {
                dto.EmployeeName = employee.Name;
            }

            return evaluationDtos;
        }

        public async Task<IEnumerable<EmployeeEvaluationDTO>> GetEvaluationsByPeriodAsync(string period)
        {
            var evaluations = await _evaluationRepository.GetByPeriodAsync(period);
            var evaluationDtos = _mapper.Map<IEnumerable<EmployeeEvaluationDTO>>(evaluations);

            foreach (var dto in evaluationDtos)
            {
                var evaluation = evaluations.FirstOrDefault(e => e.Id == dto.Id);
                dto.EmployeeName = evaluation?.Employee?.Name;
            }

            return evaluationDtos;
        }

        public async Task<EmployeeEvaluationDTO> GetLatestEvaluationByEmployeeIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {employeeId} not found.");
            }

            var evaluation = await _evaluationRepository.GetLatestByEmployeeIdAsync(employeeId);
            if (evaluation == null)
            {
                return null;
            }

            var evaluationDto = _mapper.Map<EmployeeEvaluationDTO>(evaluation);
            evaluationDto.EmployeeName = employee.Name;

            return evaluationDto;
        }

        public async Task<IEnumerable<EmployeeEvaluationDTO>> GetEvaluationsByScoreRangeAsync(int minScore, int maxScore)
        {
            var evaluations = await _evaluationRepository.GetByScoreRangeAsync(minScore, maxScore);
            var evaluationDtos = _mapper.Map<IEnumerable<EmployeeEvaluationDTO>>(evaluations);

            foreach (var dto in evaluationDtos)
            {
                var evaluation = evaluations.FirstOrDefault(e => e.Id == dto.Id);
                dto.EmployeeName = evaluation?.Employee?.Name;
            }

            return evaluationDtos;
        }

        public async Task DeleteEvaluationAsync(int id)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(id);
            if (evaluation == null)
            {
                throw new ArgumentException($"Evaluation with ID {id} not found.");
            }

            await _evaluationRepository.DeleteAsync(id);
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {employeeId} not found.");
            }

            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDTO>>(employees);
        }
    }
}