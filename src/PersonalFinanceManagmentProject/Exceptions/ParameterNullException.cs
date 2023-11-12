namespace PersonalFinanceManagmentProject.Exceptions;

[Serializable]
public class ParameterNullException : Exception, ICustomException
{
    public int StatusCode { get; }

    public ParameterNullException() { }

    public ParameterNullException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public ParameterNullException(int statusCode, string meesage, Exception inner)
        : base(meesage, inner)
    {
        StatusCode = statusCode;
    }
}