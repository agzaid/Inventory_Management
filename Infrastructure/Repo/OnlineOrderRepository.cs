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
    public class OnlineOrderRepository : Repository<OnlineOrder>, IOnlineOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OnlineOrderRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(OnlineOrder entity)
        {
            _db.OnlineOrder.Update(entity);
        }
    }
}
