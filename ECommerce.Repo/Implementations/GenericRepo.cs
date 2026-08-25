
using ECommerce.DAL.Context;
using ECommerce.DAL.Entities;
using LearnSphere.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LearnSphere.Repo.Implementations
{
    public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : BaseEntity
    {
      private readonly AppDbContext _context;
      private readonly DbSet<TEntity> dbset;
        public GenericRepo(AppDbContext context)
        {
            _context=context;
            dbset= context.Set<TEntity>();
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await dbset.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await dbset.AddRangeAsync(entities);
            return entities;
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(bool asNoTracking=false)
        {
          IQueryable<TEntity> query = dbset;
            if(asNoTracking)
                query=query.AsNoTracking();
            return await query.ToListAsync();
            
        }

        public async Task<TEntity> GetByIdAsync(int Id, bool asNoTracking = false)
        {
            IQueryable<TEntity> query = dbset;
            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id== Id);

        }

        public  Task RemoveAsync(TEntity item)
        {
            dbset.Remove(item);
            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            dbset.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public  Task UpdateAsync(TEntity entity)
        {
            dbset.Update(entity);
           
            return Task.CompletedTask;

        }
        public  Task UpdateRangeAsync(List<TEntity>entities)
        {
            dbset.UpdateRange(entities);
           
            return Task.CompletedTask;

        }
        
        
        public async Task<bool>AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbset.AnyAsync(predicate);
        }

        
    }
}
