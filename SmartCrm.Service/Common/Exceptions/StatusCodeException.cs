using System.Net;

namespace SmartCrm.Service.Common.Exceptions;

public class StatusCodeException(HttpStatusCode statusCode,
                                 string message)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; set; } = statusCode;
}
