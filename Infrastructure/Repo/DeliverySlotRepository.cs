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
    public class DeliverySlotRepository : Repository<DeliverySlot>, IDeliverySlotRepository
    {
        private readonly ApplicationDbContext _db;

        public DeliverySlotRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(DeliverySlot entity)
        {
            _db.DeliverySlot.Update(entity);
        }
    }
}
