using StockApp.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface ICustomReportService
    {
        Task<CustomReportDto> GenerateReportAsync(ReportParametersDto parameters);
        Task<IEnumerable<CustomReportDto>> GetReportHistoryAsync();
        Task<CustomReportDto> GenerateSalesReportAsync(ReportParametersDto parameters);
        Task<CustomReportDto> GenerateInventoryReportAsync(ReportParametersDto parameters);
        Task<CustomReportDto> GenerateProductPerformanceReportAsync(ReportParametersDto parameters);
        Task<CustomReportDto> GenerateCategoryReportAsync(ReportParametersDto parameters);
        Task<IEnumerable<string>> GetAvailableReportTypesAsync();
    }
}