using SmartCrm.Domain.Entities.Payments;

namespace SmartCrm.Data.Interfaces.Payments;

public interface IPayment : IRepository<Payment>
{
    Task<decimal> GetTotalAmountCollectedAsync();
    Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime date);
    Task<decimal> GetCurrentMonthTotalAmountAsync();
    Task<IEnumerable<Payment>> GetPaymentsByCurrentMonthAsync();
}
