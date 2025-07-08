using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public class MeterRequestRepository : IMeterRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public MeterRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MeterRequest>> GetPendingRequestsAsync()
        {
            return await _context.MeterRequests
                .Where(mr => mr.Status == MeterRequestStatus.Pending)
                .ToListAsync();
        }

        public async Task<MeterRequest?> GetByIdAsync(int id)
        {
            return await _context.MeterRequests.FindAsync(id);
        }

        public async Task UpdateAsync(MeterRequest meterRequest)
        {
            _context.MeterRequests.Update(meterRequest);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(MeterRequest meterRequest)
        {
            _context.MeterRequests.Add(meterRequest);
            await _context.SaveChangesAsync();
        }
    }
}
