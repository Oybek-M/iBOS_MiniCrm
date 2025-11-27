using Microsoft.AspNetCore.Http;

namespace SmartCrm.Service.Helpers;

public class HttpContextHelper
{
    public static IHttpContextAccessor Accessor;
    public static HttpContext HttpContext => Accessor?.HttpContext;
    public static IHeaderDictionary RequestHeaders => HttpContext?.Request?.Headers;
    public static IHeaderDictionary ResponseHeaders => HttpContext?.Response?.Headers;
}
