
using System.Linq.Expressions;

namespace ApiReservacionesGym.Respositorio
{
    public interface IGenericRepository<T,TId> where T : class,IEntidadBase<TId>
    {
        Task AddAsync(T entity);
        Task<int> ContarAsync(Expression<Func<T, bool>> predicate);
        void Delete(T entity);
        Task<List<T>> GetAllAsync(string includeProperties = "");
        Task<T> GetByAsyncId(TId id, string includeProperties = "");
        Task<T> GetByConditionAsync(Expression<Func<T, bool>> condition,string includeProperties = "");
        void Update(T entity);
    }
}