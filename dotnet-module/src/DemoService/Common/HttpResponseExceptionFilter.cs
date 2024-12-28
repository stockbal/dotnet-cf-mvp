// Copyright (c) Demo AG. All Rights Reserved.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevEpos.CF.Demo.Common;

class ErrorResponse {
    public ErrorContent? Error { get; set; }
}

class ErrorContent {
    public string? TaskId { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public string? InnerDetail { get; set; }

}

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter {
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context) {
        if (context.Exception is HttpResponseException httpResponseException) {
            HandleHttpResponseException(context, httpResponseException);
        } else if (context.Exception is Exception exc) {
            HandleException(context, exc);
        }
    }

    private void HandleException(ActionExecutedContext context, Exception exc) {
        var errorContent = new ErrorContent();

        FillErrorContent(errorContent, exc);
        SetContextResult(context, errorContent);
    }

    private void HandleHttpResponseException(ActionExecutedContext context, HttpResponseException httpResponseException) {
        var errorContent = new ErrorContent { TaskId = httpResponseException.TaskId };

        FillErrorContent(errorContent, httpResponseException);
        SetContextResult(context, errorContent, GetStatusCode(httpResponseException.InnerException) ?? 400);
    }

    private void SetContextResult(ActionExecutedContext context, ErrorContent errorContent, int statusCode = 400) {
        context.Result = new JsonResult(new ErrorResponse { Error = errorContent }) {
            StatusCode = statusCode,
            SerializerSettings = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }
        };
        context.ExceptionHandled = true;
    }

    private int? GetStatusCode(Exception? innerException) {
        if (innerException is ServiceException srvExc) {
            return srvExc.StatusCode ?? 400;
        }
        return 400;
    }

    private void FillErrorContent(ErrorContent errorContent, Exception exc) {
        if (exc.InnerException != null) {
            if (exc.InnerException is ServiceException innerSrvExc) {
                errorContent.Title = innerSrvExc.Message;
                errorContent.Detail = innerSrvExc.Detail;
                errorContent.InnerDetail = innerSrvExc.InnerMessage;
            } else {
                errorContent.Title = exc.InnerException.Message;
            }
        } else if (exc is ServiceException srvExc) {
            errorContent.Title = srvExc.Message;
            errorContent.Detail = srvExc.Detail;
            errorContent.InnerDetail = srvExc.InnerMessage;
        } else {
            errorContent.Title = exc.Message;
        }
    }
}
