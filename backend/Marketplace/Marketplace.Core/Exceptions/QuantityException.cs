namespace Marketplace.Core.Exceptions
{
    public class QuantityException : Exception
    {
        public QuantityException() : base() { }
        public QuantityException(string message) : base(message) { }
    }
}
