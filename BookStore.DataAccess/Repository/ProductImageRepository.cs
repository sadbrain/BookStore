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

public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly ApplicationDbContext _db;
    public ProductImageRepository(ApplicationDbContext db):base(db)
    {
        _db = db;
    }
    public void Update(ProductImage entity)
    {
        _db.ProductImages.Update(entity);
    }
}
