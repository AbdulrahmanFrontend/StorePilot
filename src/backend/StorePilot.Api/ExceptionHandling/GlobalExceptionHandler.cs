using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using StorePilot.Application.Exceptions;

namespace StorePilot.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            logger.LogError(
                exception,
                "An exception occurred after the response started. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

            return false;
        }

        // 2. الحصول على معرف التتبع الفريد لكل طلب HTTP للربط بين الخطأ في الـ Logs وشكوى العميل
        var traceId = httpContext.TraceIdentifier;

        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "Internal Server Error";
        string message = "An unexpected error occurred. Please try again later.";
        IDictionary<string, string[]>? errors = null;

        // 3. فحص نوع الاستثناء القادم لتحديد كود الـ HTTP المناسب وطبيعة الرسالة
        switch (exception)
        {
            case NotFoundException notFound:
                statusCode = StatusCodes.Status404NotFound;
                title = "Not Found";
                message = notFound.Message;
                logger.LogWarning(
                    notFound, 
                    "Not Found Exception [TraceId: {TraceId}]", 
                    traceId);
                break;

            case ConflictException conflict:
                statusCode = StatusCodes.Status409Conflict;
                title = "Conflict";
                message = conflict.Message;
                logger.LogWarning(
                    conflict, 
                    "Conflict Exception [TraceId: {TraceId}]", 
                    traceId);
                break;

            case BusinessRuleException businessRule:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Bad Request";
                message = businessRule.Message;
                logger.LogWarning(
                    businessRule, 
                    "Business Rule Exception [TraceId: {TraceId}]", 
                    traceId);
                break;

            case ValidationException validationEx:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Validation Failed";
                message = "One or more validation errors occurred.";
                // تجميع أخطاء التحقق لكل حقل على حدة لتسهيل عرضها في الواجهة الأمامية
                errors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                logger.LogWarning(
                    validationEx, 
                    "Validation Exception [TraceId: {TraceId}]", 
                    traceId);
                break;

            default:
                // الخطأ غير معروف تماماً (يُسجل كـ Error خطير)
                logger.LogError(
                    exception,
                    "Unhandled Exception [TraceId: {TraceId}]",
                    traceId);
                break;
        }

        // 4. ضبط حالة الاستجابة ونوع المحتوى إلى JSON
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        // 5. إذا كانت هناك أخطاء Validation، نرجع ValidationProblemDetails المتخصصة بالحقول
        if (errors != null)
        {
            var validationProblem = new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = title,
                Detail = message,
                Instance = httpContext.Request.Path
            };
            validationProblem.Extensions["traceId"] = traceId;

            await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
        }
        else
        {
            // 6. للأخطاء العادية، نرجع ProblemDetails القياسي
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = message,
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = traceId;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        // 7. إرجاع True تعني أننا قمنا بالتعامل مع الخطأ بنجاح ولم يعد هناك داعي لاستمرار الـ Pipeline
        return true;
    }
}
