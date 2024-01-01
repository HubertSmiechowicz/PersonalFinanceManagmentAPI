namespace PersonalFinanceManagmentProject.Exceptions
{
    [Serializable]
    public class AmountOutOfRangeException : Exception, ICustomException
    {
        public int StatusCode { get; }

        public AmountOutOfRangeException() { }

        public AmountOutOfRangeException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public AmountOutOfRangeException(int statusCode, string meesage, Exception inner)
            : base(meesage, inner)
        {
            StatusCode = statusCode;
        }
    }
}
