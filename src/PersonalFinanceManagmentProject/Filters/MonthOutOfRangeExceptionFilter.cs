using Microsoft.AspNetCore.Mvc.Filters;

namespace PersonalFinanceManagmentProject.Filters;

public class MonthOutOfRangeExceptionFilter : CustomExceptionFilter<MonthOutOfRangeException>
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