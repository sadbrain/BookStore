using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db):base(db)
    {
        _db = db;
    }
    public void Update(Product entity)
    {
        var product = _db.Products.FirstOrDefault(e => e.Id == entity.Id);
        if (product != null)
        {
            product.Title = entity.Title;
            product.ISBN = entity.ISBN;
            product.Price = entity.Price;
            product.Price50 = entity.Price50;
            product.Price100 = entity.Price100;
            product.ListPrice = entity.ListPrice;
            product.Description = entity.Description;
            product.CategoryId = entity.CategoryId;
            product.Author = entity.Author;
/*            if(product.ImageUrl != null){
                product.ImageUrl = entity.ImageUrl;
            }*/
        }
    }
}
