using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StockApp.Application.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "The Name is Required")]
        [MinLength(2)]
        [MaxLength(100)]
        [DisplayName("Name")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "The Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(100)]
        [DisplayName("Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "The Position is Required")]
        [MaxLength(50)]
        [DisplayName("Position")]
        public string Position { get; set; }
        
        [Required(ErrorMessage = "The Department is Required")]
        [MaxLength(20)]
        [DisplayName("Department")]
        public string Department { get; set; }
        
        [Required(ErrorMessage = "The Salary is Required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value")]
        [DisplayName("Salary")]
        public decimal Salary { get; set; }
        
        [Required(ErrorMessage = "The Hire Date is Required")]
        [DisplayName("Hire Date")]
        public DateTime HireDate { get; set; }
        
        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;
        
        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }
    }
}