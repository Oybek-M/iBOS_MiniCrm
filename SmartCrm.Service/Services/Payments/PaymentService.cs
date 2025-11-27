using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Entities.Payments;
using SmartCrm.Domain.Enums;
using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.Documents;
using SmartCrm.Service.Helpers;
using SmartCrm.Service.DTOs.Payments;
using SmartCrm.Service.Interfaces.Payments;
using System.Net;

namespace SmartCrm.Service.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePaymentDto> _createValidator;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreatePaymentDto> createValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        public async Task<(byte[] file, string fileName)> CreateAsync(CreatePaymentDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors));

            var student = await _unitOfWork.Student.GetById(dto.StudentId);
            if (student is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Bunday IDga ega o'quvchi topilmadi.");

            var newPayment = new Payment
            {
                StudentId = dto.StudentId,
                Amount = dto.AmountPaid,
                Type = dto.Type,
                Notes = dto.Notes,
                PaymentDate = TimeHelper.GetDateTime()
            };
            await _unitOfWork.Payment.Add(newPayment);


            student.Balance += dto.AmountPaid;

            decimal fullMonthlyPayment = student.MonthlyPaymentAmount * (1 - student.DiscountPercentage / 100.0m);

            decimal balanceWithoutCurrentMonth = student.Balance + fullMonthlyPayment;

            if (balanceWithoutCurrentMonth >= 0)
            {
                student.PaymentStatus = MonthlyPaymentStatus.Paid;
            }
            else
            {
                student.PaymentStatus = MonthlyPaymentStatus.Unpaid;
            }

            await _unitOfWork.Student.Update(student);

            var paymentDetails = await GetByIdAsync(newPayment.Id);
            var pdfDocument = new PaymentReceiptDocument(paymentDetails);
            byte[] pdfBytes = pdfDocument.GeneratePdf();
            string studentName = $"{paymentDetails.Student.FirstName}_{paymentDetails.Student.LastName}".Replace(" ", "_");
            string dateStamp = paymentDetails.PaymentDate.ToString("yyyy-MM-dd");
            string fileName = $"chek_{studentName}_{dateStamp}.pdf";
            return (file: pdfBytes, fileName: fileName);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var payment = await _unitOfWork.Payment.GetById(id);
            if (payment is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "To'lov yozuvi topilmadi.");

            var student = await _unitOfWork.Student.GetById(payment.StudentId);
            if (student != null)
            {
                student.Balance -= payment.Amount;

                decimal fullMonthlyPayment = student.MonthlyPaymentAmount * (1 - student.DiscountPercentage / 100.0m);
                decimal balanceWithoutCurrentMonth = student.Balance + fullMonthlyPayment;

                if (balanceWithoutCurrentMonth >= 0)
                {
                    student.PaymentStatus = MonthlyPaymentStatus.Paid;
                }
                else
                {
                    student.PaymentStatus = MonthlyPaymentStatus.Unpaid;
                }

                await _unitOfWork.Student.Update(student);
            }

            await _unitOfWork.Payment.Remove(payment);
            return true;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _unitOfWork.Payment.GetAll()
                .Include(p => p.Student)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> GetByIdAsync(Guid id)
        {
            var payment = await _unitOfWork.Payment.GetAll()
                .Include(p => p.Student).ThenInclude(s => s.Group) 
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "To'lov yozuvi topilmadi.");

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<decimal> GetTotalCollectedAmountAsync()
        {
            return await _unitOfWork.Payment.GetTotalAmountCollectedAsync();
        }

        public async Task<IEnumerable<PaymentDto>> GetAllForBackupAsync()
        {
            var payments = await _unitOfWork.Payment.GetAll()
                .Include(p => p.Student)
                    .ThenInclude(s => s.Group)
                        .ThenInclude(g => g.Teacher) 
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }
        public async Task<decimal> GetTotalAmountByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1).AddTicks(-1);

            var totalAmount = await _unitOfWork.Payment.GetAll()
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .SumAsync(p => p.Amount);

            return totalAmount;
        }
        public async Task<IEnumerable<PaymentDto>> GetPaymentsByDateAsync(DateTime date)
        {
            var payments = await _unitOfWork.Payment.GetPaymentsByDateAsync(date);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByMonthAsync()
        {
            var payments = await _unitOfWork.Payment.GetPaymentsByCurrentMonthAsync();

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }


        public async Task<decimal> GetCurrentMonthTotalAmountAsync()
        {
            return await _unitOfWork.Payment.GetCurrentMonthTotalAmountAsync();
        }
    }
}