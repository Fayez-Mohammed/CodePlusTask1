using ECommerce.Shared.DTOs;
using ECommerce.DAL.Entities;
using LearnSphere.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Result<Customer>> GetByIdAsync(int id);
        Task<Result<Customer>> CreateAsync(CreateCustomerDto dto);
        Task<Result<string>> UpgradeToVipAsync(int id);
    }
}
