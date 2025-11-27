using SmartCrm.Service.DTOs.Payments;

namespace SmartCrm.Service.Interfaces.Payments;

public interface IPaymentService
{
    Task<(byte[] file, string fileName)> CreateAsync(CreatePaymentDto dto);
    Task<PaymentDto> GetByIdAsync(Guid id);
    Task<IEnumerable<PaymentDto>> GetAllAsync();
    Task<decimal> GetTotalCollectedAmountAsync();
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<PaymentDto>> GetAllForBackupAsync();
    Task<decimal> GetTotalAmountByDateAsync(DateTime date);
    Task<IEnumerable<PaymentDto>> GetPaymentsByDateAsync(DateTime date);
    Task<IEnumerable<PaymentDto>> GetPaymentsByMonthAsync();
    Task<decimal> GetCurrentMonthTotalAmountAsync();
}
