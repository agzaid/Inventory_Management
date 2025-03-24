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
        public DbSet<Invoice> Order { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryLog> InventoryLog { get; set; }
        public DbSet<OnlineOrder> OnlineOrder { get; set; }
        public DbSet<InvoiceItem> InvoiceItem { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<ShippingFreight> ShippingFreight { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<DeliverySlot> DeliverySlot { get; set; }
        public DbSet<UserDeliverySlot> UserDeliverySlot { get; set; }


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
                var districts = new List<District>
                    {
                        new District { Id = 1, Name = "other" },
                    };
                context.ShippingFreight.AddRange(
                    new ShippingFreight { Area = "other", Price = 50, Districts = districts }
                );

                context.SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the composite primary key for the junction table
            modelBuilder.Entity<UserDeliverySlot>()
                .HasKey(uds => new { uds.UserId, uds.DeliverySlotId });

            // Set up the many-to-many relationship
            //modelBuilder.Entity<UserDeliverySlot>()
            //    .HasOne(uds => uds.User)
            //    .WithMany(u => u.UserDeliverySlots)
            //    .HasForeignKey(uds => uds.UserId);

            //modelBuilder.Entity<UserDeliverySlot>()
            //    .HasOne(uds => uds.DeliverySlot)
            //    .WithMany(ds => ds.UserDeliverySlots)
            //    .HasForeignKey(uds => uds.SlotId);
        }
    }
}
