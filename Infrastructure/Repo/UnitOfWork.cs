﻿using Application.Common.Interfaces;
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
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
            Category = new CategoryRepository(_db);
            Image = new ImageRepository(_db);
        }
        public void Save()
        {
             _db.SaveChanges();
        }
        //public void Dispose()
        //{
        //    _db.Dispose();
        //}
    }
}
