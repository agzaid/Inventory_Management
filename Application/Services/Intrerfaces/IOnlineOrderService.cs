﻿using Domain.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Application.Services.Intrerfaces
{
    public interface IOnlineOrderService
    {
        IEnumerable<ProductVM> GetAllProducts();
        Task<List<SelectListItem>> ForCartView();
        PortalVM GetAllProductsForPortal();
        ProductVM GetProductById(int id);
        ProductVM GetProductDetails(int id);
        Task<string[]> CreateProduct(ProductVM product);
        Task<string[]> CreateOrder(CartVM cart);
        ProductVM CreateProductForViewingInCreate();
        bool UpdateProduct(ProductVM productVM);
        bool DeleteProduct(int id);
        bool HardDeleteProduct(int id);
    }
}
