namespace BusinessLaag.Exceptions
{
    public class VraagExceptions : Exception
    {

        public VraagExceptions()
        { }

        public VraagExceptions(string message) : base(message)
        { }

        public VraagExceptions(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
