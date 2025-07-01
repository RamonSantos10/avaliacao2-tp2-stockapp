using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockApp.Application.Interfaces;
using Microsoft.Data.SqlClient;

namespace StockApp.Application.Services
{
    public class BackupService : IBackupService
    {
        private readonly string _backupPath;
        private readonly string _connectionString;
        private readonly ILogger<BackupService> _logger;
        private readonly string _databaseName;

        public BackupService(IConfiguration configuration, ILogger<BackupService> logger)
        {
            _backupPath = configuration["BackupPath"] ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StockAppBackups");
            
            // Usar a mesma lógica do Program.cs para obter a connection string
            _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                ?? configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("ConnectionString não encontrada nem em variável de ambiente nem em appsettings.json");
            
            _logger = logger;
            _databaseName = ExtractDatabaseName(_connectionString);
            
            // Criar diretório de backup se não existir
            if (!Directory.Exists(_backupPath))
            {
                Directory.CreateDirectory(_backupPath);
            }
        }

        public void BackupDatabase()
        {
            try
            {
                var backupFile = Path.Combine(_backupPath, $"backup_{DateTime.Now:yyyyMMddHHmmss}.bak");
                
                _logger.LogInformation($"Iniciando backup do banco de dados {_databaseName} para {backupFile}");

                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                var backupCommand = $@"
                    BACKUP DATABASE [{_databaseName}] 
                    TO DISK = '{backupFile}' 
                    WITH FORMAT, INIT, 
                    NAME = 'StockApp-Full Database Backup', 
                    SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                using var command = new SqlCommand(backupCommand, connection);
                command.CommandTimeout = 300; // 5 minutos de timeout
                command.ExecuteNonQuery();

                _logger.LogInformation($"Backup concluído com sucesso: {backupFile}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar backup do banco de dados");
                throw new InvalidOperationException($"Falha no backup: {ex.Message}", ex);
            }
        }

        public async Task BackupDatabaseAsync()
        {
            try
            {
                var backupFile = Path.Combine(_backupPath, $"backup_{DateTime.Now:yyyyMMddHHmmss}.bak");
                
                _logger.LogInformation($"Iniciando backup assíncrono do banco de dados {_databaseName} para {backupFile}");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var backupCommand = $@"
                    BACKUP DATABASE [{_databaseName}] 
                    TO DISK = '{backupFile}' 
                    WITH FORMAT, INIT, 
                    NAME = 'StockApp-Full Database Backup', 
                    SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                using var command = new SqlCommand(backupCommand, connection);
                command.CommandTimeout = 300; // 5 minutos de timeout
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation($"Backup assíncrono concluído com sucesso: {backupFile}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar backup assíncrono do banco de dados");
                throw new InvalidOperationException($"Falha no backup assíncrono: {ex.Message}", ex);
            }
        }

        public string GetBackupPath()
        {
            return _backupPath;
        }

        public bool ValidateBackupPath()
        {
            try
            {
                if (!Directory.Exists(_backupPath))
                {
                    Directory.CreateDirectory(_backupPath);
                }
                
                // Testar se é possível escrever no diretório
                var testFile = Path.Combine(_backupPath, "test_write.tmp");
                System.IO.File.WriteAllText(testFile, "test");
                System.IO.File.Delete(testFile);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao validar caminho de backup: {_backupPath}");
                return false;
            }
        }

        private string ExtractDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
    }
}