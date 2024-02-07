// <copyright file="HttpResponseExceptionFilter.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Middleware
{
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    // Copied from https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-5.0#use-exceptions-to-modify-the-response

    /// <inheritdoc />
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <inheritdoc />
        public int Order => int.MaxValue - 10;

        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException exception)
            {
                context.Result = new ObjectResult(exception.ResponseBody)
                {
                    StatusCode = (int?)exception.StatusCode,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
