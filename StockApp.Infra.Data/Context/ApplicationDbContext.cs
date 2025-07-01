using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Infra.Data.Context
{
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
                : base(options)
            { }
            public DbSet<Category> Categories { get; set; }
            public virtual DbSet<Product> Products { get; set; }
            public DbSet<Feedback> Feedbacks { get; set; }
            public DbSet<AnonymousFeedback> AnonymousFeedbacks { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<Return> Returns { get; set; }
            public DbSet<Employee> Employees { get; set; }
            public DbSet<EmployeeEvaluation> EmployeeEvaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);
                builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); 
                builder.Entity<Feedback>().ToTable("Feedbacks");
                builder.Entity<AnonymousFeedback>().ToTable("AnonymousFeedbacks");
                builder.Entity<Employee>().ToTable("Employees");
                builder.Entity<EmployeeEvaluation>().ToTable("EmployeeEvaluations");
                
                // Configuração do relacionamento Employee -> EmployeeEvaluation
                builder.Entity<EmployeeEvaluation>()
                    .HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
}