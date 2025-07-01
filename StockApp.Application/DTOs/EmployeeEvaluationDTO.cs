using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StockApp.Application.DTOs
{
    public class EmployeeEvaluationDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "The Employee ID is Required")]
        [DisplayName("Employee ID")]
        public int EmployeeId { get; set; }
        
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Score is Required")]
        [Range(0, 100, ErrorMessage = "Evaluation Score must be between 0 and 100")]
        [DisplayName("Evaluation Score")]
        public int EvaluationScore { get; set; }
        
        [Required(ErrorMessage = "The Feedback is Required")]
        [MinLength(10, ErrorMessage = "Feedback must be at least 10 characters")]
        [MaxLength(1000)]
        [DisplayName("Feedback")]
        public string Feedback { get; set; }
        
        [MaxLength(500)]
        [DisplayName("Goals")]
        public string Goals { get; set; }
        
        [MaxLength(500)]
        [DisplayName("Achievements")]
        public string Achievements { get; set; }
        
        [MaxLength(500)]
        [DisplayName("Areas for Improvement")]
        public string AreasForImprovement { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Date is Required")]
        [DisplayName("Evaluation Date")]
        public DateTime EvaluationDate { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Period is Required")]
        [MaxLength(20)]
        [DisplayName("Evaluation Period")]
        public string EvaluationPeriod { get; set; }
        
        [Required(ErrorMessage = "The Evaluated By field is Required")]
        [MaxLength(100)]
        [DisplayName("Evaluated By")]
        public string EvaluatedBy { get; set; }
        
        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }
        
        [DisplayName("Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
    
    public class CreateEmployeeEvaluationDTO
    {
        [Required(ErrorMessage = "The Employee ID is Required")]
        public int EmployeeId { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Score is Required")]
        [Range(0, 100, ErrorMessage = "Evaluation Score must be between 0 and 100")]
        public int EvaluationScore { get; set; }
        
        [Required(ErrorMessage = "The Feedback is Required")]
        [MinLength(10, ErrorMessage = "Feedback must be at least 10 characters")]
        [MaxLength(1000)]
        public string Feedback { get; set; }
        
        [MaxLength(500)]
        public string Goals { get; set; }
        
        [MaxLength(500)]
        public string Achievements { get; set; }
        
        [MaxLength(500)]
        public string AreasForImprovement { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Date is Required")]
        public DateTime EvaluationDate { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Period is Required")]
        [MaxLength(20)]
        public string EvaluationPeriod { get; set; }
        
        [Required(ErrorMessage = "The Evaluated By field is Required")]
        [MaxLength(100)]
        public string EvaluatedBy { get; set; }
    }
    
    public class UpdateEmployeeEvaluationDTO
    {
        [Required(ErrorMessage = "The Employee ID is Required")]
        public int EmployeeId { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Score is Required")]
        [Range(0, 100, ErrorMessage = "Evaluation Score must be between 0 and 100")]
        public int EvaluationScore { get; set; }
        
        [Required(ErrorMessage = "The Feedback is Required")]
        [MinLength(10, ErrorMessage = "Feedback must be at least 10 characters")]
        [MaxLength(1000)]
        public string Feedback { get; set; }
        
        [MaxLength(500)]
        public string Goals { get; set; }
        
        [MaxLength(500)]
        public string Achievements { get; set; }
        
        [MaxLength(500)]
        public string AreasForImprovement { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Date is Required")]
        public DateTime EvaluationDate { get; set; }
        
        [Required(ErrorMessage = "The Evaluation Period is Required")]
        [MaxLength(20)]
        public string EvaluationPeriod { get; set; }
        
        [Required(ErrorMessage = "The Evaluated By field is Required")]
        [MaxLength(100)]
        public string EvaluatedBy { get; set; }
    }
}