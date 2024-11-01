using System.Runtime.InteropServices;
using PandaKidsServer.Decrypter;

namespace Tests;

public class TestDecrypter
{
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestDecrypt2() {
        var key = new byte[20];
        key[0] = 1;
        key[1] = 2;
        key[2] = 3;
        
        var buffer = new byte[20];
        buffer[0] = 1;
        buffer[1] = 2;
        buffer[2] = 3;
        var ptr = StreamDecrypter.DecryptBuffer(key, key.Length, buffer, buffer.Length);
        var reversedString = Marshal.PtrToStringAnsi(ptr);
        StreamDecrypter.ReleaseBuffer(ptr.ToInt64());
        Console.WriteLine("ret: " + reversedString);
    }
}