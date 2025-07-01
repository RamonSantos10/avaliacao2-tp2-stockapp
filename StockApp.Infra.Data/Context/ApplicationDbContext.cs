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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Return> Returns { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
