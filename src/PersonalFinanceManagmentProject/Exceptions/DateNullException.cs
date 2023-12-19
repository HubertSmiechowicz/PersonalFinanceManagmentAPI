namespace PersonalFinanceManagmentProject.Exceptions;

[Serializable]
public class DateNullException : Exception, ICustomException
{
    public int StatusCode { get; }
    
    public int TransactionId { get; }

    public DateNullException() { }

    public DateNullException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
    
    public DateNullException(int statusCode, int transactionId, string message)
        : base(message)
    {
        StatusCode = statusCode;
        TransactionId = transactionId;
    }

    public DateNullException(int statusCode, string meesage, Exception inner)
        : base(meesage, inner)
    {
        StatusCode = statusCode;
    }
}