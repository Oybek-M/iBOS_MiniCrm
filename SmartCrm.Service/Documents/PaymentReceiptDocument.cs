using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartCrm.Service.DTOs.Payments;
using SmartCrm.Service.Helpers;

namespace SmartCrm.Service.Documents
{
    public class PaymentReceiptDocument : IDocument
    {
        private readonly PaymentDto _payment;

        public PaymentReceiptDocument(PaymentDto payment)
        {
            _payment = payment;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(80, 200, Unit.Millimetre);

                    page.Margin(5, Unit.Millimetre);

                    page.DefaultTextStyle(x => x.FontSize(16).FontFamily(Fonts.Courier));

                    page.Content().Column(column =>
                    {
                        column.Item().AlignCenter().Text("TO'LOV CHEKI").Bold().FontSize(16);
                        column.Item().PaddingTop(10);

                        var uzbekistanPaymentDate = TimeHelper.ToUzbekistanTime(_payment.PaymentDate);
                        column.Item().Text($"Sana: {uzbekistanPaymentDate:dd.MM.yyyy HH:mm}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("O'quvchi ma'lumotlari:").SemiBold();
                        column.Item().PaddingLeft(5).Text($"To'liq ism: {_payment.Student.FirstName} {_payment.Student.LastName}");
                        column.Item().PaddingLeft(5).Text($"Telefon: {_payment.Student.PhoneNumber}");
                        column.Item().PaddingTop(10);

                        column.Item().Text("To'lov tafsilotlari:").SemiBold();
                        column.Item().PaddingLeft(5).Text(text =>
                        {
                            text.Span("Summa: ");
                            text.Span($"{_payment.Amount:N0} so'm").Bold();
                        });
                        column.Item().PaddingLeft(5).Text($"To'lov turi: {_payment.Type}");

                        if (!string.IsNullOrEmpty(_payment.Notes))
                        {
                            column.Item().PaddingLeft(5).Text($"Izoh: {_payment.Notes}");
                        }
                    });
                });
        }
    }
}