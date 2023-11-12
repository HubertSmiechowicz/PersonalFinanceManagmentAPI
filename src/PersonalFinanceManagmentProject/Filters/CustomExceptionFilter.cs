using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PersonalFinanceManagmentProject.Exceptions;

namespace PersonalFinanceManagmentProject.Filters;

public class CustomExceptionFilter<T> : IActionFilter, IOrderedFilter where T : Exception, ICustomException
{ 
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is T exception)
        {
            var error = new ErrorDetails(exception.StatusCode, exception.Message);
            context.HttpContext.Response.StatusCode = error.StatusCode;
            context.Result = new JsonResult(error);
        }
        context.ExceptionHandled = true;
    }
}