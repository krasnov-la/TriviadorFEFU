using System.Linq.Expressions;

namespace DataAccess.Repository;

public interface IRepository<T> where T : class 
{
    void Add(T entity);
    IEnumerable<T> All();
    IQueryable<T> Where(Expression<Func<T, bool>> filter);
    T? First(Expression<Func<T, bool>> filter);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}