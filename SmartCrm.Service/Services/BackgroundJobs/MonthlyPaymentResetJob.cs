using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCrm.Service.BackgroundJobs
{
    public class MonthlyPaymentResetJob : BackgroundService
    {
        private readonly ILogger<MonthlyPaymentResetJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeZoneInfo _uzbekistanTimeZone;

        public MonthlyPaymentResetJob(ILogger<MonthlyPaymentResetJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            _uzbekistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Oylik to'lov statusini yangilovchi fon xizmati ishga tushdi.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    DateTime nowInUzbekistan = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _uzbekistanTimeZone);

                    DateTime startOfCurrentMonth = new DateTime(nowInUzbekistan.Year, nowInUzbekistan.Month, 1);
                    DateTime nextRunTimeInUzbekistan = startOfCurrentMonth.AddMonths(1).AddMinutes(1);

                    TimeSpan delay = nextRunTimeInUzbekistan - nowInUzbekistan;

                    if (delay.TotalMilliseconds <= 0)
                    {
                        nextRunTimeInUzbekistan = nextRunTimeInUzbekistan.AddMonths(1);
                        delay = nextRunTimeInUzbekistan - nowInUzbekistan;
                    }

                    _logger.LogInformation("Keyingi to'lov statusi yangilanishi: {runTime} (O'zbekiston vaqti bilan)", nextRunTimeInUzbekistan);

                    await Task.Delay(delay, stoppingToken);

                    await ResetPaymentStatusesAsync(stoppingToken);
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Oylik to'lov statusini yangilashda kutilmagan xatolik yuz berdi.");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }

        private async Task ResetPaymentStatusesAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("O'quvchilar balansidan oylik to'lovlarni yechib olish boshlandi...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var allStudents = await unitOfWork.Student.GetAll().ToListAsync(stoppingToken);

                if (allStudents.Any())
                {
                    foreach (var student in allStudents)
                    {
                        decimal monthlyFee = student.MonthlyPaymentAmount * (1 - student.DiscountPercentage / 100.0m);

                        student.Balance -= monthlyFee;

                        if (student.Balance >= 0)
                        {
                            student.PaymentStatus = MonthlyPaymentStatus.Paid;
                        }
                        else
                        {
                            student.PaymentStatus = MonthlyPaymentStatus.Unpaid;
                        }

                        await unitOfWork.Student.Update(student);
                    }
                    _logger.LogInformation("{count} ta o'quvchining balansi yangi oy uchun yangilandi.", allStudents.Count);
                }
                else
                {
                    _logger.LogInformation("O'quvchilar topilmadi.");
                }
            }
        }
    }
}