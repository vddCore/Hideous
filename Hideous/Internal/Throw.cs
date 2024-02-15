namespace Hideous.Internal
{
    internal static class Throw
    {
        internal static class If
        {
            internal static void ReportIdMismatch(byte expectedReportId, byte providedReportId, string what)
            {
                if (expectedReportId != providedReportId)
                {
                    throw new HideousException(
                        $"Report ID mismatch. This report ID: '{expectedReportId}', provided {what} specifies: '{providedReportId}'."
                    );
                }
            }

            internal static void ReportBufferTooLong(int maxLength, int length, string what)
            {
                if (length >= maxLength)
                {
                    throw new HideousException(
                        $"Report buffer too large. Maximum length of the report supported by the device: {maxLength}, provided {what} buffer is {length} byte(s) long."
                    );
                }
            }

            internal static void DataBufferTooLong(int maxLength, int length)
            {
                if (length + 1 >= maxLength)
                {
                    throw new HideousException(
                        $"Report data buffer too large. Maximum length of data based on the report length supported by the device: {maxLength - 1}, provided data buffer is {length} byte(s) long."
                    );
                }
            }
        }
    }
}