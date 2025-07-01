using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApp.Domain.Entities
{
    public class EmployeeEvaluation
    {
        public int Id { get; set; }
        
        [Required]
        public int EmployeeId { get; set; }
        
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        
        [Required]
        [Range(0, 100)]
        public int EvaluationScore { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Feedback { get; set; }
        
        [MaxLength(500)]
        public string Goals { get; set; }
        
        [MaxLength(500)]
        public string Achievements { get; set; }
        
        [MaxLength(500)]
        public string AreasForImprovement { get; set; }
        
        [Required]
        public DateTime EvaluationDate { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string EvaluationPeriod { get; set; } // "Q1 2025", "2024", etc.
        
        [Required]
        [MaxLength(100)]
        public string EvaluatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}