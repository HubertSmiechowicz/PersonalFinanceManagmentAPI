namespace PersonalFinanceManagmentProject.Exceptions;

[Serializable]
public class BadStatusException : Exception, ICustomException
{
    public int StatusCode { get; }

    public BadStatusException() { }

    public BadStatusException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public BadStatusException(int statusCode, string meesage, Exception inner)
        : base(meesage, inner)
    {
        StatusCode = statusCode;
    }
    
}