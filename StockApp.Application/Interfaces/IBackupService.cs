namespace StockApp.Application.Interfaces
{
    public interface IBackupService
    {
        void BackupDatabase();
        Task BackupDatabaseAsync();
        string GetBackupPath();
        bool ValidateBackupPath();
    }
}