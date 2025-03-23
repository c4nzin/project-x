using System.Text.Json;
using FluentValidation;
using AutoWrapper.Wrappers;

namespace src.Core;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public ValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering(); 


        //body boşsa direkt işlem yapmadan next yapıyoruz hehrangi bir validasyona gerek yok
        if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
        {
            await _next(context);
            return;
        }

        try
        {
            var bodyType = GetRequestTypeFromRoute(context);

            if (bodyType != null) 
            {
                context.Request.Body.Position = 0;
                var requestBody = await JsonSerializer.DeserializeAsync(context.Request.Body, bodyType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (requestBody != null)
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(bodyType);
                    var validator = _serviceProvider.GetService(validatorType) as IValidator;

                    if (validator != null)
                    {
                        var result = await validator.ValidateAsync(new ValidationContext<object>(requestBody));

                        if (!result.IsValid)
                        {
                            var errors = result.Errors.Select(e => e.ErrorMessage);

                            throw new ApiException($"errors : {errors}");
                        }
                    }
                }
                context.Request.Body.Position = 0;
            }
        }
        catch (ApiException)
        {
            throw; //Burada api exceptiondan için
        }
        catch (Exception ex)  //burada apiexception dışında bir exception yakalaması için 
        {
            throw new ApiException($"Invalid request body or validation middleware error: {ex}");
        }

        await _next(context); //delegate ile chaini devam ettiriyoruz.
    } 

    private Type? GetRequestTypeFromRoute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null) return null;

        var handlerMethod = endpoint.Metadata.OfType<Delegate>().FirstOrDefault()?.Method;

        if (handlerMethod == null) return null;

        var bodyParam = handlerMethod.GetParameters().FirstOrDefault(p =>
          !p.ParameterType.IsPrimitive &&
          p.ParameterType != typeof(string) &&
          !p.ParameterType.Namespace!.StartsWith("Microsoft"));

        return bodyParam?.ParameterType;
    }
}
