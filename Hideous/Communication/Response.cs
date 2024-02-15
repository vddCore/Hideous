namespace Hideous.Communication
{
    public abstract record Response
    {
        protected Response(byte[] data)
        {
            var ms = new MemoryStream(data);

            try
            {
                Parse(ms);
            }
            finally
            {
                try
                {
                    ms.Dispose();
                }
                catch { /* Ignore */ }
            }
        }

        protected abstract void Parse(MemoryStream stream);
    }
}