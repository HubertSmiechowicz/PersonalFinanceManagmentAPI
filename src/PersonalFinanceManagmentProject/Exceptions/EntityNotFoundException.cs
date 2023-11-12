namespace PersonalFinanceManagmentProject.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : Exception, ICustomException
    {
        public int StatusCode { get; }
        public int EntityId { get; }

        public EntityNotFoundException() { }

        public EntityNotFoundException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public EntityNotFoundException(int statusCode, string meesage, Exception inner)
            : base(meesage, inner)
        {
            StatusCode = statusCode;
        }

        public EntityNotFoundException(int statusCode, string message, int entityId)
            : base(message)
        {
            StatusCode = statusCode;
            EntityId = entityId;
        }
    }
}
