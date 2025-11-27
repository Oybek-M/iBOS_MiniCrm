using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCrm.Service.DTOs.Payments;
using SmartCrm.Service.Interfaces.Payments;
using SmartCrm.Service.Services.BackUpService;

namespace SmartCrm.Controllers.Common
{
    [Route("api/payments")]
    [ApiController]
    [Authorize(Roles = "SuperAdministrator, Administrator")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly BackupService _backupService; 

        public PaymentsController(IPaymentService paymentService, BackupService backupService)
        {
            _paymentService = paymentService;
            _backupService = backupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePaymentDto dto)
        {
            var (pdfFile, fileName) = await _paymentService.CreateAsync(dto);
            return File(pdfFile, "application/pdf", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var payments = await _paymentService.GetPaymentsByMonthAsync();
            return Ok(payments);
        }

        [HttpGet("total-collected-amount")]
        [Authorize(Roles = "SuperAdministrator")]
        public async Task<IActionResult> GetCurrentMonthTotalAmountAsync() 
        {
            var totalAmount = await _paymentService.GetCurrentMonthTotalAmountAsync();

            return Ok(new
            {
                Month = DateTime.Now.ToString("MMMM, yyyy"),
                TotalAmount = totalAmount
            });
        }
        [HttpGet("current-month")] 
        public async Task<IActionResult> GetCurrentMonthPaymentsAsync()
        {
            var payments = await _paymentService.GetPaymentsByMonthAsync();
            return Ok(payments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            return Ok(payment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _paymentService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("download-daily-report")]
        public async Task<IActionResult> DownloadDailyReportAsync([FromBody] DateReportDto dto)
        {
            var (fileBytes, fileName) = await _backupService.GenerateDailyPaymentsReportAsBytesAsync(dto.Date);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}