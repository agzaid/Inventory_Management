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
    public class SellerRepository : Repository<Seller>, ISellerRepository
    {
        private readonly ApplicationDbContext _db;

        public SellerRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Seller entity)
        {
            _db.Seller.Update(entity);
        }
    }
}
