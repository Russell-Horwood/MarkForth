namespace MarkForth
{
    public class MarkForthException : Exception
    {
        protected MarkForthException(uint hResult)
        {
            this.setHResult(hResult);
        }

        protected MarkForthException(uint hResult, string message)
            : base(message)
        {
            this.setHResult(hResult);
        }

        protected MarkForthException(uint hResult, string message, Exception innerException)
            : base(message, innerException)
        {
            this.setHResult(hResult);
        }

        private void setHResult(uint value)
        {
            this.HResult = unchecked((int)value);
        }
    }
}
