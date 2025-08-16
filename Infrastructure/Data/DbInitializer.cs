using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void SeedData(ApplicationDbContext context)
        {
            if (!context.Category.Any())
            {
                context.Category.Add(new Category { CategoryName = "rice" });
                context.SaveChanges();
            }

            if (!context.DeliverySlot.Any())
            {
                context.DeliverySlot.AddRange(
                    new DeliverySlot { StartTime = "7", EndTime = "8", AM_PM = "pm" },
                    new DeliverySlot { StartTime = "8", EndTime = "9", AM_PM = "pm" }
                );
                context.SaveChanges();
            }

            if (!context.ShippingFreight.Any())
            {
                var districts = new List<District>
                {
                    new District { Name = "other", Price = 50 }
                };

                context.ShippingFreight.Add(
                    new ShippingFreight { ShippingArea = "other", Price = 50, Districts = districts }
                );
                context.SaveChanges();
            }
        }
    }
}

