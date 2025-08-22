using Application.Common.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IImageRepository Image { get; private set; }
        public IInvoiceRepository Invoice { get; private set; }
        public IShippingFreightRepository ShippingFreight { get; private set; }
        public ICustomerRepository Customer { get; private set; }
        public IDeliverySlotRepository DeliverySlot { get; private set; }
        public IDistrictRepository District { get; private set; }
        public IOnlineOrderRepository OnlineOrder { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IFeedbackRepository Feedback { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
            Category = new CategoryRepository(_db);
            Image = new ImageRepository(_db);
            Invoice = new InvoiceRepository(_db);
            ShippingFreight = new ShippingFreightRepository(_db);
            Customer = new CustomerRepository(_db);
            DeliverySlot = new DeliverySlotRepository(_db);
            District = new DistrictRepository(_db);
            OnlineOrder = new OnlineOrderRepository(_db);
            Brand = new BrandRepository(_db);
            Feedback = new FeedbackRepository(_db);
        }
        public async Task SaveAsync()
        {
             await _db.SaveChangesAsync();
        }
        //public void Dispose()
        //{
        //    _db.Dispose();
        //}
    }
}
