using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using SmartCrm.Service.Helpers;
using System.Data;
using OfficeOpenXml;
using System.Diagnostics;
using SmartCrm.Service.Interfaces.Payments;
using OfficeOpenXml.Style;

namespace SmartCrm.Service.Services.BackUpService
{
    public class BackupService : BackgroundService
    {
        private readonly ILogger<BackupService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostEnvironment _env;

        private readonly int _intervalDays;
        private readonly string _backupFolder;
        private readonly bool _readableBackup;
        private readonly int _maxFiles;

        public BackupService(
            ILogger<BackupService> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IHostEnvironment env)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _env = env;

            _intervalDays = _configuration.GetValue<int>("BackupSettings:BackupIntervalDays");
            _readableBackup = _configuration.GetValue<bool>("BackupSettings:ReadableBackup");
            _maxFiles = _configuration.GetValue<int>("BackupSettings:MaxBackupFilesToKeep");

            var relativeFolder = _configuration.GetValue<string>("BackupSettings:BackupFolder");
            _backupFolder = Path.Combine(_env.ContentRootPath, relativeFolder);

            if (!Directory.Exists(_backupFolder))
                Directory.CreateDirectory(_backupFolder);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Avtomatik Backup xizmati ishga tushdi.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Keyingi avtomatik backup {days} kundan so'ng.", _intervalDays);
                    await Task.Delay(TimeSpan.FromDays(_intervalDays), stoppingToken);
                    await PerformFullBackupAndNotifyAsync("Avtomatik");
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex) { _logger.LogError(ex, "Avtomatik backup jarayonida kutilmagan xatolik."); }
            }
        }

        public async Task PerformFullBackupAndNotifyAsync(string triggerSource = "Manual")
        {
            _logger.LogInformation("To'liq backup jarayoni boshlandi (Manba: {source})", triggerSource);
            var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            var filePaths = new List<string>();

            var sqlBackupPath = Path.Combine(_backupFolder, $"db_backup_{timestamp}.sql");
            try
            {
                await CreateSqlDumpAsync(sqlBackupPath);
                filePaths.Add(sqlBackupPath);
            }
            catch (Exception ex) { _logger.LogError(ex, "SQL dump yaratishda xatolik."); }

            if (_readableBackup)
            {
                var excelBackupPath = Path.Combine(_backupFolder, $"readable_backup_{timestamp}.xlsx");
                try
                {
                    await CreateReadableTablesBackupAsync(excelBackupPath);
                    filePaths.Add(excelBackupPath);
                }
                catch (Exception ex) { _logger.LogError(ex, "O'qiladigan Excel yaratishda xatolik."); }
            }

            var paymentsBackupPath = Path.Combine(_backupFolder, $"payments_backup_{timestamp}.xlsx");
            try
            {
                await CreatePaymentsBackupAsync(paymentsBackupPath);
                filePaths.Add(paymentsBackupPath);
            }
            catch (Exception ex) { _logger.LogError(ex, "To'lovlar backup'ini yaratishda xatolik."); }

            CleanOldBackups();
            await SendFilesToTelegramAsync(filePaths, triggerSource);
        }

        public async Task<(byte[] file, string fileName)> GeneratePaymentsBackupAsBytesAsync()
        {
            _logger.LogInformation("To'lovlar tarixi Excel fayli (download uchun) generatsiya qilinmoqda.");
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = $"payments_history_{timestamp}.xlsx";

            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var payments = await paymentService.GetAllForBackupAsync();

            ExcelPackage.License.SetNonCommercialPersonal("My NonCommercial organization");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("To'lovlar Tarixi");

            ws.Cells[1, 1].Value = "Sana";
            ws.Cells[1, 2].Value = "O'quvchi F.I.Sh";
            ws.Cells[1, 3].Value = "Guruh";
            ws.Cells[1, 4].Value = "O'qituvchi"; 
            ws.Cells[1, 5].Value = "Summa";
            ws.Cells[1, 6].Value = "To'lov Turi";
            ws.Cells[1, 7].Value = "Izoh";

            using (var range = ws.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 2;
            foreach (var payment in payments)
            {
                var uzbekistanTime = TimeHelper.ToUzbekistanTime(payment.PaymentDate);
                ws.Cells[row, 1].Value = uzbekistanTime.ToString("dd.MM.yyyy HH:mm");
                ws.Cells[row, 2].Value = $"{payment.Student.FirstName} {payment.Student.LastName}";
                ws.Cells[row, 3].Value = payment.Student.Group?.Name ?? "Noma'lum";
                ws.Cells[row, 4].Value = payment.Student.Group?.Teacher != null
                    ? $"{payment.Student.Group.Teacher.FirstName} {payment.Student.Group.Teacher.LastName}"
                    : "Noma'lum";
                ws.Cells[row, 5].Value = payment.Amount;
                ws.Cells[row, 6].Value = payment.Type.ToString();
                ws.Cells[row, 7].Value = payment.Notes;
                row++;
            }

            ws.Column(5).Style.Numberformat.Format = "#,##0";
            ws.Cells.AutoFitColumns();
            var fileBytes = await package.GetAsByteArrayAsync();
            _logger.LogInformation("Excel fayli xotirada muvaffaqiyatli yaratildi.");
            return (file: fileBytes, fileName: fileName);
        }

        public async Task PerformPaymentsOnlyBackupAndNotifyAsync()
        {
            _logger.LogInformation("Faqat to'lovlar backup'i boshlandi.");
            var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            var paymentsBackupPath = Path.Combine(_backupFolder, $"payments_only_{timestamp}.xlsx");

            try
            {
                await CreatePaymentsBackupAsync(paymentsBackupPath);
                CleanOldBackups();
                await SendFilesToTelegramAsync(new List<string> { paymentsBackupPath }, "Faqat to'lovlar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faqat to'lovlar backup'ini yaratishda xatolik.");
            }
        }



        private async Task CreateSqlDumpAsync(string sqlBackupPath)
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");
            var pgBuilder = new NpgsqlConnectionStringBuilder(connString);
            var pgDumpPath = _configuration.GetValue<string>("BackupSettings:PgDumpPath") ?? "pg_dump";
            var pgUri = $"postgresql://{pgBuilder.Username}:{pgBuilder.Password}@{pgBuilder.Host}:{pgBuilder.Port}/{pgBuilder.Database}";

            var psi = new ProcessStartInfo
            {
                FileName = pgDumpPath,
                Arguments = $"--dbname=\"{pgUri}\" --format=plain --no-owner --no-privileges --file=\"{sqlBackupPath}\"",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            var stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception($"pg_dump xatosi: {stderr}");

            _logger.LogInformation("SQL dump muvaffaqiyatli yaratildi: {file}", sqlBackupPath);
        }

        private async Task CreateReadableTablesBackupAsync(string excelFilePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("My NonCommercial organization");
            await using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            var tables = new List<string>();
            var getTablesCmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';", conn);
            await using (var reader = await getTablesCmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync()) tables.Add(reader.GetString(0));
            }

            using var package = new ExcelPackage();
            foreach (var table in tables)
            {
                var ws = package.Workbook.Worksheets.Add(table);
                var selectCmd = new NpgsqlCommand($@"SELECT * FROM public.""{table}"";", conn);
                await using var dr = await selectCmd.ExecuteReaderAsync();
                var dt = new DataTable();
                dt.Load(dr);
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells.AutoFitColumns();
            }
            await using var stream = File.Create(excelFilePath);
            await package.SaveAsAsync(stream);
            _logger.LogInformation("O'qiladigan Excel backup muvaffaqiyatli yaratildi: {file}", excelFilePath);
        }

        private async Task CreatePaymentsBackupAsync(string filePath)
        {
            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var payments = await paymentService.GetAllForBackupAsync();

            ExcelPackage.License.SetNonCommercialPersonal("My NonCommercial organization");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("To'lovlar Tarixi");

            ws.Cells[1, 1].Value = "Sana";
            ws.Cells[1, 2].Value = "O'quvchi F.I.Sh";
            ws.Cells[1, 3].Value = "Guruh";
            ws.Cells[1, 4].Value = "O'qituvchi"; 
            ws.Cells[1, 5].Value = "Summa";
            ws.Cells[1, 6].Value = "To'lov Turi";
            ws.Cells[1, 7].Value = "Izoh";

            using (var range = ws.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 2;
            foreach (var payment in payments)
            {
                var uzbekistanTime = TimeHelper.ToUzbekistanTime(payment.PaymentDate);
                ws.Cells[row, 1].Value = uzbekistanTime.ToString("dd.MM.yyyy HH:mm");
                ws.Cells[row, 2].Value = $"{payment.Student.FirstName} {payment.Student.LastName}";
                ws.Cells[row, 3].Value = payment.Student.Group?.Name ?? "Noma'lum";

                ws.Cells[row, 4].Value = payment.Student.Group?.Teacher != null
                    ? $"{payment.Student.Group.Teacher.FirstName} {payment.Student.Group.Teacher.LastName}"
                    : "Noma'lum";

                ws.Cells[row, 5].Value = payment.Amount;
                ws.Cells[row, 6].Value = payment.Type.ToString();
                ws.Cells[row, 7].Value = payment.Notes;
                row++;
            }

            ws.Column(5).Style.Numberformat.Format = "#,##0";
            ws.Cells.AutoFitColumns();

            await using var stream = File.Create(filePath);
            await package.SaveAsAsync(stream);
            _logger.LogInformation("To'lovlar tarixi Excel fayli muvaffaqiyatli yaratildi: {file}", filePath);
        }

        private async Task SendFilesToTelegramAsync(List<string> filePaths, string triggerSource)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var bot = scope.ServiceProvider.GetRequiredService<TelegramBotHelper>();
                var caption = $"🤖 *Backup ({triggerSource})* \n\nSana: `{DateTime.Now:dd.MM.yyyy HH:mm}`";

                foreach (var path in filePaths)
                {
                    if (File.Exists(path))
                    {
                        await bot.SendBackupFileAsync(path, caption);
                        caption = ""; 
                    }
                }
                _logger.LogInformation("Backup fayllari Telegramga yuborildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Backup fayllarini Telegramga yuborishda xatolik.");
            }
        }

        private void CleanOldBackups()
        {
            try
            {
                var files = new DirectoryInfo(_backupFolder)
                    .GetFiles()
                    .Where(f => f.Extension == ".sql" || f.Extension == ".xlsx")
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();

                if (files.Count <= _maxFiles) return;

                foreach (var f in files.Skip(_maxFiles))
                {
                    f.Delete();
                    _logger.LogInformation("Eskirgan backup o‘chirildi: {file}", f.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eskirgan backuplarni tozalashda xatolik.");
            }
        }
        public async Task<(byte[] file, string fileName)> GenerateDailyPaymentsReportAsBytesAsync(DateTime date)
        {
            _logger.LogInformation("{date} sanasi uchun kunlik to'lovlar hisoboti (Excel) generatsiya qilinmoqda.", date.ToString("dd.MM.yyyy"));

            var dateStamp = date.ToString("yyyy-MM-dd");
            var fileName = $"daily_payments_{dateStamp}.xlsx";

            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var payments = await paymentService.GetPaymentsByDateAsync(date);

            ExcelPackage.License.SetNonCommercialPersonal("My NonCommercial organization");
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add($"{date:dd.MM.yyyy} To'lovlari");

            ws.Cells[1, 1].Value = "Sana";
            ws.Cells[1, 2].Value = "O'quvchi F.I.Sh";
            ws.Cells[1, 3].Value = "Guruh";
            ws.Cells[1, 4].Value = "O'qituvchi";
            ws.Cells[1, 5].Value = "Summa";
            ws.Cells[1, 6].Value = "To'lov Turi";
            ws.Cells[1, 7].Value = "Izoh";

            using (var range = ws.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
            }

            int row = 2;
            foreach (var payment in payments)
            {
                var uzbekistanTime = TimeHelper.ToUzbekistanTime(payment.PaymentDate);
                ws.Cells[row, 1].Value = uzbekistanTime.ToString("dd.MM.yyyy HH:mm");
                ws.Cells[row, 2].Value = $"{payment.Student.FirstName} {payment.Student.LastName}";
                ws.Cells[row, 3].Value = payment.Student.Group?.Name ?? "Noma'lum";
                ws.Cells[row, 4].Value = payment.Student.Group?.Teacher != null
                    ? $"{payment.Student.Group.Teacher.FirstName} {payment.Student.Group.Teacher.LastName}"
                    : "Noma'lum";
                ws.Cells[row, 5].Value = payment.Amount;
                ws.Cells[row, 6].Value = payment.Type.ToString();
                ws.Cells[row, 7].Value = payment.Notes;
                row++;
            }

            ws.Cells[row + 1, 4].Value = "JAMI:";
            ws.Cells[row + 1, 4].Style.Font.Bold = true;
            ws.Cells[row + 1, 5].Formula = $"SUM(E2:E{row - 1})";
            ws.Cells[row + 1, 5].Style.Font.Bold = true;
            ws.Cells[row + 1, 5].Style.Numberformat.Format = "#,##0";
            ws.Column(5).Style.Numberformat.Format = "#,##0";
            ws.Cells.AutoFitColumns();
            var fileBytes = await package.GetAsByteArrayAsync();
            _logger.LogInformation("Kunlik Excel fayli xotirada muvaffaqiyatli yaratildi.");
            return (file: fileBytes, fileName: fileName);
        }
    }
}