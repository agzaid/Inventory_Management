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
    public class BrandsCategoriesRepository : Repository<BrandsCategories>, IBrandsCategoriesRepository
    {
        private readonly ApplicationDbContext _db;

        public BrandsCategoriesRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(BrandsCategories entity)
        {
            _db.BrandsCategories.Update(entity);
        }
    }
}
