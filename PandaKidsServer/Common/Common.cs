using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace PandaKidsServer.Common;

public static class Common
{
    public static string GetCurrentTime() {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
    }

    public static long GetCurrentTimestamp() {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    public static string Md5String(string input) {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var t in hashBytes) sb.Append(t.ToString("x2"));
        return sb.ToString();
    }

    public static bool IsEmpty(string? value) {
        return value == null || value.Length <= 0;
    }

    public static int AsInt(string? value) {
        try {
            return int.Parse(value!);
        }
        catch (Exception) {
            return int.MinValue;
        }
    }

    public static bool IsValidInt(int value) {
        return value != int.MinValue;
    }

    public static bool DeleteFile(string filePath) {
        try  {
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
            else {
                Console.WriteLine("File does not exist.");
            }
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }

    public static bool DeleteFolder(string folderPath) {
        try  {
            if (Directory.Exists(folderPath))  {
                Directory.Delete(folderPath, true);
            }
            else {
                Console.WriteLine("Folder does not exist.");
            }
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }
}