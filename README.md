## Hideous
Ugly stuff made simple, at least.

Hideous is a library for .NET allowing users to interface with HID devices using a simple programming interface that doesn't require them to guess "how the fuck do I use it".

### I'm a fuckwit - examples?
So am I, but I get you. Catch.

#### Enumerating certain HID devices
```Csharp
using Hideous;

var collection = new HidDeviceCollection(
  vendorId: 0x0B05,
  productId: 0x19B6
);

foreach (var device in collection) {
  Console.WriteLine(device.Properties);
}
```

#### Enumerating all HID devices
```Csharp
using Hideous;

var collection = new HidDeviceCollection(); /* Same as (0, 0) */

foreach (var device in collection) {
  Console.WriteLine(device.Properties);
}
```

#### Looking up a device by usage page
```Csharp
using Hideous;

var collection = new HidDeviceCollection(0x0B05, 0x19B6);
var myDevice = collection.FirstOrDefault(x => x.Properties.UsagePage == 0xFF00);
```

#### Enumerating reports of a device
```Csharp
using Hideous;

var collection = new HidDeviceCollection(0x0B05, 0x19B6);
var myDevice = collection.FirstOrDefault(
    x => x.Properties.UsagePage == 0x0001
      && x.Properties.UsageId == 0x0006
);

if (myDevice != null)
{
    myDevice.Connect();
    
    /* Descriptor only becomes valid AFTER connection was established. */
    foreach (var report in myDevice.Descriptor.Reports)
    {
        Console.WriteLine(report);
    }
}
```

#### Reading a HID input report that has an ID
```CSharp
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
```

## Notes
The library currently supports Windows and Linux only - because of the natives extracted when the library first runs, you will find either `hidapi.dll` or `hidapi.so` if not already present.

Requires HidApi >= 0.14.0.