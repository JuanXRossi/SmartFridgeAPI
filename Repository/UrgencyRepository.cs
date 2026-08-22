using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Dtos.Urgency;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Models;
using SmartFridgeAPI.Models.Data;

namespace SmartFridgeAPI.Repository
{
    public class UrgencyRepository : IUrgencyRepository
    {
        private readonly ApplicationDBContext _context;
        public UrgencyRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Urgency> CreateAsync(Urgency urgencyModel)
        {
            await _context.Urgencies.AddAsync(urgencyModel);
            await _context.SaveChangesAsync();

            return urgencyModel;
        }

        public async Task<Urgency?> DeleteAsync(int id)
        {
            var urgency = await _context.Urgencies.FirstOrDefaultAsync(u => u.Id == id);

            if (urgency == null)
            {
                return null;
            }

            _context.Urgencies.Remove(urgency);
            await _context.SaveChangesAsync();

            return urgency;
        }

        public async Task<bool> ExistsAsync(int urgencyId)
        {
            return await _context.Urgencies.AnyAsync(u => u.Id == urgencyId);
        }

        public async Task<List<UrgencyDto>> GetAllAsync()
        {
            return await _context.Urgencies
                .Select(u => new UrgencyDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    MinAmount = u.MinAmount
                }).ToListAsync();
        }

        public async Task<Urgency?> GetByIdAsync(int id)
        {
            return await _context.Urgencies.FindAsync(id);
        }

        public async Task<Urgency?> UpdateAsync(int id, UpdateUrgencyRequestDto urgencyDto)
        {
            var urgencyModel = await _context.Urgencies.FirstOrDefaultAsync(u => u.Id == id);

            if (urgencyModel == null)
            {
                return null;
            }

            urgencyModel.Name = urgencyDto.Name!;
            urgencyModel.MinAmount = urgencyDto.MinAmount;

            await _context.SaveChangesAsync();

            return urgencyModel;
        }
    }
}