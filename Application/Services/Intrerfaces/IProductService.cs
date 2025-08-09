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
        Task<IEnumerable<ProductVM>> GetAllProducts();
        Task<string[]> CreateProduct(ProductVM product);
        ProductVM CreateProductForViewingInCreate();
        ProductVM CreateProductForViewingInCreate(ProductVM productVM);
        Task<bool> UpdateProduct(ProductVM productVM);
        Task<bool> HardDeleteProduct(int id);
        Task<bool> DeleteProduct(int id);
        Task<IEnumerable<ProductVM>> GetAllProductsForPortal();
        Task<ProductVM> GetProductById(int id);
        Task<ProductVM> GetProductDetails(int id);
    }
}
