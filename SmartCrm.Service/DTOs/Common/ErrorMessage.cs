namespace SmartCrm.Service.DTOs.Common
{
    public class ErrorMessage
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}