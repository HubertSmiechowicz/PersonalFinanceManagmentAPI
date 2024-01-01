﻿using Microsoft.AspNetCore.Mvc.Filters;
using PersonalFinanceManagmentProject.Exceptions;

namespace PersonalFinanceManagmentProject.Filters
{
    public class AmountOutOfRangeExceptionFilter : CustomExceptionFilter<AmountOutOfRangeException>
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            this.OnActionExecuting(context);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            this.OnActionExecuted(context);
        }
    }
}
