using ECommerce.DAL.Context;
using ECommerce.DAL.Entities;
using LearnSphere.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnSphere.Repo.Implementations
{
    public  class UnitOfWork : IUnitOfWork
    {
        AppDbContext _context;
        Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

        }
        public  IGenericRepo<T> Repository<T>() where T : BaseEntity
        {
            var type=typeof(T);
            if (!repositories.ContainsKey(type))
            {
                var repo =new GenericRepo<T>(_context);
                repositories.Add(type, repo);

            }

          return (IGenericRepo<T>) repositories[type];
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
