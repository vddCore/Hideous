using Hideous;

var deviceFamily = new HidDeviceCollection(0x331A, 0x5018);

byte[]? descriptor  = null;
foreach (var device in deviceFamily.Devices)
{
    Console.WriteLine(device.Properties);

    if (device.Properties.UsagePage == 0xFF04)
    {
        device.Connect();
        descriptor = device.ReadRawReportDescriptor();
        device.Disconnect();
    }
}

// if (descriptor != null)
// {
//     for (var i = 0; i < descriptor.Length; i++)
//     {
//         if (i != 0 && i % 16 == 0)
//         {
//             Console.Write("\n");
//         }
//         
//         Console.Write(descriptor[i].ToString("X2"));
//         Console.Write(" ");
//     }
// }