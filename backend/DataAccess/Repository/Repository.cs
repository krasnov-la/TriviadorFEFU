using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DataAccess.Models;

namespace DataAccess.Repository;
public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;
    public Repository(AppDbContext db)
    {
        // Type typeT = typeof(T);
        // if(typeT == typeof(Question)) db.IncludeQuestionRelations();
        // db.IncludeQuestionRelations();
        _dbSet = db.Set<T>();
    }
    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public IEnumerable<T> All()
    {
        return _dbSet.ToList();
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = _dbSet;
        return query.Where(filter);
    }

    public T? First(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = _dbSet;
        return query.Where(filter).FirstOrDefault();
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}