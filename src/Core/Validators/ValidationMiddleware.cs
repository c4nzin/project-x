using System.Text.Json;
using AutoWrapper.Wrappers;
using FluentValidation;
using src.Utils;

namespace src.Core;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public ValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
        {
            await _next(context);
        }

        try
        {
            Type? bodyType = GetRequest.GetRequestTypeFromRoute(context);

            if (bodyType != null)
            {
                context.Request.Body.Position = 0;
                object? requestBody = await JsonSerializer.DeserializeAsync(
                    context.Request.Body,
                    bodyType,
                    _jsonSerializerOptions
                );

                if (requestBody != null)
                {
                    Type? validatorType = typeof(IValidator<>).MakeGenericType(bodyType);
                    IValidator validator = _serviceProvider.GetService(validatorType) as IValidator;

                    if (validator != null)
                    {
                        var result = await validator.ValidateAsync(
                            new ValidationContext<object>(requestBody)
                        );

                        if (!result.IsValid)
                        {
                            IEnumerable<string> errors = result.Errors.Select(e => e.ErrorMessage);

                            throw new ApiException($"errors : {errors}");
                        }
                    }
                }
                context.Request.Body.Position = 0;
            }
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApiException($"Invalid request body or validation middleware error: {ex}");
        }

        await _next(context);
    }
}
