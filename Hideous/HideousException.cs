namespace Hideous
{
    public class HideousException : Exception
    {
        public HideousException(string? message)
            : base(message)
        {
        }

        public HideousException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}