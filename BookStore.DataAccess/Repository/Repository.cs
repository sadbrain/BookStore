using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;
    public Repository(ApplicationDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>();
    }
    public void Add(T entity)
    {
        _db.Add(entity);
    }

	public T Get(Expression<Func<T, bool>> filter, String? includeProperties = null, bool tracked = false)
	{
		IQueryable<T> query;
		if (tracked)
		{
			query = dbSet;
		}
		else
		{
			query = dbSet.AsNoTracking();
		}
		query = query.Where(filter);
		if (!string.IsNullOrEmpty(includeProperties))
		{
			foreach (var includePro in includeProperties
			 .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				//query ở ddaaya là một phần tử product
				query = query.Include(includePro);
			}
		}
		return query.FirstOrDefault();
	}

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query.ToList();
    }
    public void Remove(T entity)
    {
        _db.Remove(entity);

    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _db.RemoveRange(entities);

    }
}
