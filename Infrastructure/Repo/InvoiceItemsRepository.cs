using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class InvoiceItemsRepository : Repository<InvoiceItem>, IInvoiceItemRepository
    {
        private readonly ApplicationDbContext _db;

        public InvoiceItemsRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(InvoiceItem entity)
        {
            _db.InvoiceItem.Update(entity);
        }
    }
}
