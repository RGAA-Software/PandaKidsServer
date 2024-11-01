using System.Runtime.InteropServices;

namespace PandaKidsServer.Common;

public class MemoryClean {
    
    [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
    public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
    
    public static void ClearMemory() {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }
    }
}