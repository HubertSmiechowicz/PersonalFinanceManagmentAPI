namespace PersonalFinanceManagmentProject.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public int EntityId { get; }

        public EntityNotFoundException() { }

        public EntityNotFoundException(string message)
            : base(message) { }

        public EntityNotFoundException(string meesage, Exception inner)
            : base(meesage, inner) { }

        public EntityNotFoundException(string message, int entityId)
            : this(message)
        {
            EntityId = entityId;
        }
    }
}
