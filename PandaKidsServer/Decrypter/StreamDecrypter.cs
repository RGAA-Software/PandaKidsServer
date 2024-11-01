using System.Runtime.InteropServices;

namespace PandaKidsServer.Decrypter;

public class StreamDecrypter
{
    
    [DllImport("PandaKidsDecrypter.dll", EntryPoint = "DecryptBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DecryptBuffer(byte[] key, int keyLength, byte[] buffer, int bufferLength);
    
    [DllImport("PandaKidsDecrypter.dll", EntryPoint = "ReleaseBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ReleaseBuffer(long ptr);

}