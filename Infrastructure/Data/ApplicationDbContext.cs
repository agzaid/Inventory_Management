using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<InventoryLog> InventoryLog { get; set; }
        public DbSet<Invoice> Order { get; set; }
        public DbSet<InvoiceItem> InvoiceItem { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<ShippingFreight> ShippingFreight { get; set; }

        public static void SeedData(ApplicationDbContext context)
        {
            // Check if the data is already seeded to avoid duplication
            if (!context.Category.Any())
            {
                context.Category.AddRange(
                    new Category { CategoryName = "rice" }
                );

                context.SaveChanges();
            }
            // Check if the data is already seeded to avoid duplication
            if (!context.ShippingFreight.Any())
            {
                context.ShippingFreight.AddRange(
                    new ShippingFreight { Area = "other", Region = "other", Price = 50}
                );

                context.SaveChanges();
            }
        }
    }
}
