using ECommerce.DAL.Entities;
using LearnSphere.Repo.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnSphere.Repo.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();
        IGenericRepo<T> Repository<T> () where T : BaseEntity;
    }
}
