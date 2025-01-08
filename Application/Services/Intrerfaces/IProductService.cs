using Domain.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IProductService
    {
        IEnumerable<ProductVM> GetAllProducts();
        ProductVM GetProductById(int id);
        Task<string[]> CreateProduct(ProductVM product);
        ProductVM CreateProductForViewingInCreate();
        bool UpdateProduct(ProductVM productVM);
        bool DeleteProduct(int id);
        bool HardDeleteProduct(int id);
    }
}
