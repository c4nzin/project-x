namespace src.Utils;

public static class GetRequest
{
    public static Type? GetRequestTypeFromRoute(HttpContext httpContext)
    {
        Endpoint? endpoint = httpContext.GetEndpoint();

        if (endpoint == null)
            return null;

        var handlerMethod = endpoint.Metadata.OfType<Delegate>().FirstOrDefault()?.Method;

        if (handlerMethod == null)
            return null;

        var bodyParam = handlerMethod
            .GetParameters()
            .FirstOrDefault(p => !p.ParameterType.IsPrimitive);

        return bodyParam?.ParameterType;
    }
}
