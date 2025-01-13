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
        catch (Exception) {
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

    public static string GetFileName(string absPath) {
        return Path.GetFileName(absPath);
    }

    public static string GetFileExtension(string absPath) {
        return Path.GetExtension(absPath);
    }

    public static string GetFileNameWithoutExtension(string absPath) {
        return Path.GetFileNameWithoutExtension(absPath);
    }

    public delegate bool OnDirectoryCallback(string path);
    public delegate bool OnFileCallback(string path);
    
    public static void TraverseDirectory(string currentDirectory, OnDirectoryCallback directoryCallback)  {
        // string[] files = Directory.GetFiles(currentDirectory);
        // foreach (string file in files)  {
        //     Console.WriteLine("File: " + file);
        // }
        //
        var subDirectories = Directory.GetDirectories(currentDirectory);
        foreach (var subDir in subDirectories)  {
            if (directoryCallback.Invoke(subDir)) {
                break;
            }

            TraverseDirectory(subDir, directoryCallback);
        }
    }
    
    public static void Traverse1LevelDirectories(string path, OnDirectoryCallback cbk) {
        var subDirectories = Directory.GetDirectories(path);
        foreach (var subDir in subDirectories)  {
            if (cbk.Invoke(subDir)) {
                break;
            }
        }
    }

    public static void Traverse1LevelFiles(string path, OnFileCallback cbk) {
        var files = Directory.GetFiles(path);
        foreach (var file in files) {
            if (cbk.Invoke(file)) {
                break;
            }
        }
    }

    // 2008-09-04
    public static string GetDateByDay() {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }
    
    // 2008-9-4 20:02:10
    public static string GetTime() {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static string GetTimestampStr() {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
    }

    public static long GetTimestamp() {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }
    
    public static List<int> GenerateUniqueRandomNumbers(int min, int max)  {
        if (max - min + 1 <= 0)  {
            throw new ArgumentException("Range must be at least 5 numbers");
        }
        Random random = new Random();
        HashSet<int> uniqueNumbers = new HashSet<int>();

        while (uniqueNumbers.Count < 5)  {
            int randomNumber = random.Next(min, max + 1);
            uniqueNumbers.Add(randomNumber);
        }
        return [..uniqueNumbers];
    }
    
}