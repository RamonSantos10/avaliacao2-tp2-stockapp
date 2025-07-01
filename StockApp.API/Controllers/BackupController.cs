using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IBackupService backupService, ILogger<BackupController> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza backup síncrono do banco de dados
        /// </summary>
        /// <returns>Resultado do backup</returns>
        [HttpPost("sync")]
        public IActionResult BackupSync()
        {
            try
            {
                _logger.LogInformation("Iniciando backup síncrono via API");
                _backupService.BackupDatabase();
                
                return Ok(new 
                { 
                    message = "Backup realizado com sucesso!", 
                    timestamp = DateTime.Now,
                    backupPath = _backupService.GetBackupPath()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar backup síncrono via API");
                return StatusCode(500, new 
                { 
                    message = "Erro interno do servidor ao realizar backup", 
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Realiza backup assíncrono do banco de dados
        /// </summary>
        /// <returns>Resultado do backup</returns>
        [HttpPost("async")]
        public async Task<IActionResult> BackupAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando backup assíncrono via API");
                await _backupService.BackupDatabaseAsync();
                
                return Ok(new 
                { 
                    message = "Backup assíncrono realizado com sucesso!", 
                    timestamp = DateTime.Now,
                    backupPath = _backupService.GetBackupPath()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar backup assíncrono via API");
                return StatusCode(500, new 
                { 
                    message = "Erro interno do servidor ao realizar backup assíncrono", 
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Obtém informações sobre o serviço de backup
        /// </summary>
        /// <returns>Informações do backup</returns>
        [HttpGet("info")]
        public IActionResult GetBackupInfo()
        {
            try
            {
                var backupPath = _backupService.GetBackupPath();
                var isValidPath = _backupService.ValidateBackupPath();
                
                var backupFiles = Directory.Exists(backupPath) 
                    ? Directory.GetFiles(backupPath, "*.bak")
                        .Select(f => new 
                        {
                            fileName = Path.GetFileName(f),
                            size = new FileInfo(f).Length,
                            createdAt = System.IO.File.GetCreationTime(f)
                        })
                        .OrderByDescending(f => f.createdAt)
                        .Take(10)
                        .Cast<object>()
                        .ToList()
                    : new List<object>();

                return Ok(new 
                {
                    backupPath = backupPath,
                    isValidPath = isValidPath,
                    totalBackups = backupFiles.Count,
                    recentBackups = backupFiles,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações de backup");
                return StatusCode(500, new 
                { 
                    message = "Erro interno do servidor ao obter informações de backup", 
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Valida se o caminho de backup está configurado corretamente
        /// </summary>
        /// <returns>Status da validação</returns>
        [HttpGet("validate")]
        public IActionResult ValidateBackupPath()
        {
            try
            {
                var isValid = _backupService.ValidateBackupPath();
                var backupPath = _backupService.GetBackupPath();
                
                return Ok(new 
                {
                    isValid = isValid,
                    backupPath = backupPath,
                    message = isValid ? "Caminho de backup válido" : "Caminho de backup inválido",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar caminho de backup");
                return StatusCode(500, new 
                { 
                    message = "Erro interno do servidor ao validar caminho de backup", 
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }
    }
}