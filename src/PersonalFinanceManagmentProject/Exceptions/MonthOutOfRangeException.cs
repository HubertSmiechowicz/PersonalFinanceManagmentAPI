using PersonalFinanceManagmentProject.Exceptions;

namespace PersonalFinanceManagmentProject.Filters;

[Serializable]
public class MonthOutOfRangeException : Exception, ICustomException
{
    public int StatusCode { get; }
    
    public MonthOutOfRangeException() { }

    public MonthOutOfRangeException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public MonthOutOfRangeException(int statusCode, string meesage, Exception inner)
        : base(meesage, inner)
    {
        StatusCode = statusCode;
    }
}