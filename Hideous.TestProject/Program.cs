using Hideous;

var dev = new TestDevice();
Console.Write("a");

class TestDevice : Device
{
    public TestDevice() : base(new(0x331A, 0x5018, 0xFF01))
    {
    }
}

