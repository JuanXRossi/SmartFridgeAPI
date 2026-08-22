using SmartFridgeAPI.Dtos.Urgency;
using SmartFridgeAPI.Models;

namespace SmartFridgeAPI.Interfaces
{
    public interface IUrgencyRepository
    {
        Task<List<UrgencyDto>> GetAllAsync();

        Task<Urgency?> GetByIdAsync(int id);

        Task<Urgency> CreateAsync(Urgency urgencyModel);

        Task<Urgency?> UpdateAsync(int id, UpdateUrgencyRequestDto urgencyDto);

        Task<Urgency?> DeleteAsync(int id);

        Task<bool> ExistsAsync(int urgencyId);
    }
}