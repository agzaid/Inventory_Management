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
    public class DistrictRepository : Repository<District>, IDistrictRepository
    {
        private readonly ApplicationDbContext _db;

        public DistrictRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(District entity)
        {
            _db.District.Update(entity);
        }
    }
}
