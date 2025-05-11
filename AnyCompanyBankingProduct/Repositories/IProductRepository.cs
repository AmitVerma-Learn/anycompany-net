using System;
using System.Collections.Generic;
using AnyCompanyBankingProduct.Models;

namespace AnyCompanyBankingProduct.Repositories
{
    /// <summary>
    /// Repository interface for product data access
    /// </summary>
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int id);
        List<string> GetProductTypes();
        bool TestConnection();
    }
}
