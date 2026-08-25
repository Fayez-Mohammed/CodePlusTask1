
using ECommerce.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LearnSphere.Repo.Interfaces
{
    public interface IGenericRepo<T> where T : BaseEntity
    {
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(List<T> entities);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task<T> GetByIdAsync(int Id,bool asNoTracking=false);
        Task<IReadOnlyList<T>> GetAllAsync(bool asNoTracking=false);
     
        Task<bool> AnyAsync(Expression<Func<T,bool>> predicate);

    }
}
