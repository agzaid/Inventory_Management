using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Customer> Customer { get; set; }
        //public DbSet<Invoice> Order { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryLog> InventoryLog { get; set; }
        public DbSet<OnlineOrder> OnlineOrder { get; set; }
        public DbSet<InvoiceItem> InvoiceItem { get; set; }
        public DbSet<Seller> Seller { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<ShippingFreight> ShippingFreight { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<DeliverySlot> DeliverySlot { get; set; }
        public DbSet<UserDeliverySlot> UserDeliverySlot { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<BrandsCategories> BrandsCategories { get; set; }
        //public DbSet<BrandsCategories> BrandsCategories { get; set; }
        public DbSet<Feedback> Feedback { get; set; }


        //public static void SeedData(ApplicationDbContext context)
        //{
        //    // Check if the data is already seeded to avoid duplication
        //    if (!context.Category.Any())
        //    {
        //        context.Category.AddRange(
        //            new Category { CategoryName = "rice" }
        //        );

        //        context.SaveChanges();
        //    }
        //    if (!context.DeliverySlot.Any())
        //    {
        //        context.DeliverySlot.AddRange(
        //            new DeliverySlot { StartTime = "7", EndTime = "8", AM_PM = "pm" },
        //            new DeliverySlot { StartTime = "8", EndTime = "9", AM_PM = "pm" }
        //        );

        //        context.SaveChanges();
        //    }
        //    // Check if the data is already seeded to avoid duplication
        //    if (!context.ShippingFreight.Any())
        //    {
        //        var districts = new List<District>
        //            {
        //                new District {  Name = "other" ,Price = 50},
        //            };
        //        context.ShippingFreight.AddRange(
        //            new ShippingFreight { ShippingArea = "other", Price = 50, Districts = districts }
        //        );

        //        context.SaveChanges();
        //    }
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the composite primary key for the junction table
            modelBuilder.Entity<UserDeliverySlot>()
                .HasKey(uds => new { uds.CustomerId, uds.DeliverySlotId });

            // Set up the many-to-many relationship
            //modelBuilder.Entity<UserDeliverySlot>()
            //    .HasOne(uds => uds.User)
            //    .WithMany(u => u.UserDeliverySlots)
            //    .HasForeignKey(uds => uds.UserId);

            //modelBuilder.Entity<UserDeliverySlot>()
            //    .HasOne(uds => uds.DeliverySlot)
            //    .WithMany(ds => ds.UserDeliverySlots)
            //    .HasForeignKey(uds => uds.SlotId);

            //modelBuilder.Entity<Brand>()
            //        .HasMany(p => p.Categories)
            //        .WithMany(c => c.Brands)
            //        .UsingEntity(j => j.ToTable("BrandsCategories"));

        }
    }
}
