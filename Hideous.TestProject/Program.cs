using Hideous;

var deviceFamily = new HidDeviceCollection(0, 0);
foreach (var dev in deviceFamily)
{
    Console.WriteLine(dev.Properties);
}