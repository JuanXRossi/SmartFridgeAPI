using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupermarketShopListAPI.Dtos.Urgency;
using SupermarketShopListAPI.Models;

namespace SupermarketShopListAPI.Interfaces
{
    public interface IUrgencyRepository
    {
        Task<List<Urgency>> GetAllAsync();

        Task<Urgency?> GetByIdAsync(int id);

        Task<Urgency> CreateAsync(Urgency urgencyModel);

        Task<Urgency?> UpdateAsync(int id, UpdateUrgencyRequestDto urgencyDto);

        Task<Urgency?> DeleteAsync(int id);

        Task<bool> ExistsAsync(int urgencyId);
    }
}