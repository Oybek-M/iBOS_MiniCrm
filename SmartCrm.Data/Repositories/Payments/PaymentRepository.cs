using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces.Payments;
using SmartCrm.Domain.Entities.Payments;

namespace SmartCrm.Data.Repositories.Payments
{
    public class PaymentRepository : Repository<Payment>, IPayment
    {
        private readonly AppDbContext _appDb;
        public PaymentRepository(AppDbContext appDb) : base(appDb)
        {
            _appDb = appDb;
        }

        public async Task<decimal> GetTotalAmountCollectedAsync()
        {
            if (!await _appDb.Payments.AnyAsync())
            {
                return 0;
            }

            var totalAmount = await _appDb.Payments.SumAsync(payment => payment.Amount);

            return totalAmount;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1).AddTicks(-1);

            return await _appDb.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .Include(p => p.Student)
                    .ThenInclude(s => s.Group)
                        .ThenInclude(g => g.Teacher) 
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
        public async Task<decimal> GetCurrentMonthTotalAmountAsync()
        {
            var now = DateTime.UtcNow;

            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var totalAmount = await _appDb.Payments
                .Where(p => p.PaymentDate >= firstDayOfMonth && p.PaymentDate < firstDayOfNextMonth)
                .SumAsync(p => p.Amount);

            return totalAmount;
        }
        public async Task<IEnumerable<Payment>> GetPaymentsByCurrentMonthAsync()
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            return await _appDb.Payments
                .Where(p => p.PaymentDate >= firstDayOfMonth && p.PaymentDate < firstDayOfNextMonth)
                .Include(p => p.Student) 
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }   
}