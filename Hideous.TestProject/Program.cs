using Hideous;

var collection = new HidDeviceCollection(0x0B05, 0x19B6);
var myDevice = collection.FirstOrDefault(
    x => x.Properties.UsagePage == 0xFF31
         && x.Properties.UsageId == 0x0076
);

if (myDevice != null)
{
    myDevice.Connect();

    while (true)
    {
        var bytes = new byte[31];

        /**
         * 0x01 refers to the report ID,
         * not its index in the collection.
         *
         * By default reads are non-blocking.
         * To enforce blocking readsset
         * myDevice.IsReadBlocking to `true`.
         *
         * This method doesn't require prefixing
         * the data bytes with the report ID.
         *
         * The output byte array will be trimmed to the
         * actual count of bytes read from the device.
         *
         * Same deal with Feature reports, but they have
         * Set and Get methods accordingly.
         **/
        var readBytes = myDevice.Descriptor.InputReports[0x5D].Read(bytes);
        foreach (var b in readBytes)
        {
            Console.WriteLine(b.ToString("X2"));
        }
    }
}