using Hideous;

var deviceFamily = new HidDeviceCollection(0x331A, 0x5018);

foreach (var device in deviceFamily.Devices)
{
    Console.WriteLine(device.Properties);
    device.Connect();
    foreach (var report in device.Descriptor.Reports)
    {
        Console.WriteLine(report.ToString());
    }
    device.Disconnect();
}