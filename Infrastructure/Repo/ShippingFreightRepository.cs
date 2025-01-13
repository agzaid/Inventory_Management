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
    public class ShippingFreightRepository : Repository<ShippingFreight>, IShippingFreightRepository
    {
        private readonly ApplicationDbContext _db;

        public ShippingFreightRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(ShippingFreight entity)
        {
            _db.ShippingFreight.Update(entity);
        }
    }
}
