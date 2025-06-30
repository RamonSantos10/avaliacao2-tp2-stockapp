using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StockApp.Application.DTOs
{
    public class ReportParametersDto
    {
        [Required(ErrorMessage = "Report Type is required")]
        [DisplayName("Report Type")]
        public string ReportType { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Category ID")]
        public int? CategoryId { get; set; }

        [DisplayName("Product ID")]
        public int? ProductId { get; set; }

        [DisplayName("Include Details")]
        public bool IncludeDetails { get; set; } = false;

        [DisplayName("Group By")]
        public string GroupBy { get; set; }
    }

    public class CustomReportDto
    {
        public string Title { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public string ReportType { get; set; }
        public List<ReportDataDto> Data { get; set; } = new List<ReportDataDto>();
        public ReportSummaryDto Summary { get; set; }
    }

    public class ReportDataDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime? Date { get; set; }
    }

    public class ReportSummaryDto
    {
        public int TotalRecords { get; set; }
        public decimal TotalValue { get; set; }
        public string Currency { get; set; } = "BRL";
        public Dictionary<string, object> AdditionalMetrics { get; set; } = new Dictionary<string, object>();
    }
}