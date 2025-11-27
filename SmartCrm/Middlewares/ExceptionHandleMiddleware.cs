using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.DTOs.Common;
using Newtonsoft.Json;

namespace SmartCrm.Middlewares
{
    public class ExceptionHandleMiddleware(RequestDelegate requestDelegate)
    {
        private readonly RequestDelegate _request = requestDelegate;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _request(context);
            }
            catch (StatusCodeException except)
            {
                await ClientErrorHandleAsync(context, except);
            }
            catch (Exception exception)
            {
                await ServerErrorHandleAsync(context, exception);
            }
        }

        public async Task ClientErrorHandleAsync(HttpContext context, StatusCodeException exception)
        {
            context.Response.ContentType = "application/json";
            var result = new ErrorMessage()
            {
                Message = exception.Message,
                StatusCode = (int)exception.StatusCode
            };

            context.Response.StatusCode = result.StatusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        public async Task ServerErrorHandleAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var result = new ErrorMessage()
            {
                Message = exception.Message,
                StatusCode = 500
            };

            context.Response.StatusCode = result.StatusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }

}