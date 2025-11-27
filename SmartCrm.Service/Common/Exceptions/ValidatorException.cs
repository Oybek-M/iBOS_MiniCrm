using System.Net;

namespace SmartCrm.Service.Common.Exceptions;

public class ValidatorException : StatusCodeException
{
    public ValidatorException(string message) : base(HttpStatusCode.BadRequest, message) { }
}
