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

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _db;
    public OrderHeaderRepository(ApplicationDbContext db):base(db)
    {
        _db = db;
    }
    public void Update(OrderHeader entity)
    {
        _db.OrderHeaders.Update(entity);
    }
}
