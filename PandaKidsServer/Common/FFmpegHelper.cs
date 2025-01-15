using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using FFMpegCore;
using Serilog;

namespace PandaKidsServer.Common;

public static class FFmpegHelper
{
    /// <summary>
    /// Snapshot a video
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="time"> in seconds</param>
    /// <returns></returns>
    public static string? GenerateThumbnail(string inputPath, string outputPath = "", int time = 20) {
        inputPath = inputPath.Replace("\\", "/");
        
        if (outputPath.Length <= 0) {
            var outputName = Common.GetFileNameWithoutExtension(inputPath) + ".png";
            var outputFolder = Common.GetFolder(inputPath);
            if (outputFolder == null) {
                return null;
            }
            outputFolder = outputFolder.Replace("\\", "/");
            outputPath = outputFolder + "/" + outputName;
        }

        var ok = FFMpeg.Snapshot(inputPath, outputPath, null, TimeSpan.FromSeconds(time));
        if (!ok) {
            Console.WriteLine("Snapshot fialed!");
        }
        return outputPath;
    }
}