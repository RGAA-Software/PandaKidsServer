using Serilog;

namespace PandaKidsServer;

public class Settings
{
    public int ListenPort { get; set; }

    public string ResPath { get; set; } = "";

    public void CheckParams() {
        if (ListenPort == 0) {
            ListenPort = 9988;
        }
        if (!Directory.Exists(ResPath)) {
            Log.Warning("Res path not exists, clear it!");
            ResPath = "";
        }
    }

    public string Dump() {
        return "ListenPort: " + ListenPort + ", ResPath: " + ResPath;
    }
}