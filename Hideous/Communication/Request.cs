namespace Hideous.Communication
{
    public abstract class Request
    {
        public byte ReportId => Data[0];
        public byte[] Data { get; protected set; } = new byte[1];
    }

    public abstract class Request<T> : Request
        where T : Request
    {
        private int _currentDataIndex = 1;

        protected Request(byte reportId, int length, params byte[] data)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    "Request length must be at least 1."
                );
            }

            Data = new byte[length];
            Data[0] = reportId;

            if (data.Length > 0)
            {
                if (_currentDataIndex >= Data.Length)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(data),
                        "Your request length does not allow for initial data to be appended."
                    );
                }

                AppendData(data);
            }
        }

        public T AppendData(params byte[] data)
            => AppendData(out _, data);

        public T AppendData(out int bytesWritten, params byte[] data)
        {
            bytesWritten = 0;

            for (var i = 0;
                 i < data.Length && _currentDataIndex < Data.Length - 1;
                 i++, bytesWritten++, _currentDataIndex++)
            {
                if (_currentDataIndex > Data.Length - 1)
                    break;

                Data[_currentDataIndex] = data[i];
            }

            return (this as T)!;
        }
    }
}