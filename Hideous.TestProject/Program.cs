using Hideous;

var deviceFamily = new HidDeviceCollection(0, 0);

foreach (var device in deviceFamily.Devices)
{
    Console.WriteLine(device.Properties);
    device.Connect();
    device.Disconnect();
}