using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Repositories;

    public class ConsumptionRepository : GenericRepository<Consumption>,IConsumptionRepository
    {
        private readonly ApplicationDbContext _context;

    
        public ConsumptionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Consumption>> GetByMeterIdAsync(int meterId)
        {
            return await _dbSet
                .Where(c => c.MeterId == meterId)
                .ToListAsync();
        }

        public async Task<bool> AnyByMeterIdAsync(int meterId)
        {
            return await _dbSet.AnyAsync(c => c.MeterId == meterId);
        }

        public override async Task<IEnumerable<Consumption>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Meter)
                .ToListAsync();
        }

        public override async Task<Consumption> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Meter)
                .Include(c => c.TariffBracket)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    
    }
