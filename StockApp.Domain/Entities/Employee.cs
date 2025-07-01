using System;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Position { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Department { get; set; }
        
        public decimal Salary { get; set; }
        
        public DateTime HireDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}