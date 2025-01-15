using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;
using Size = OpenCvSharp.Size;

namespace PandaKidsServer.Common;

public class ImageHelper
{
    // Only for windows
    public static Image? ZoomPicture(Image sourceImage, float M, float N) {
        int targetWidth = (int)(M * sourceImage.Width);
        int targetHeight = (int)(N * sourceImage.Height);
        try  {
            ImageFormat format = sourceImage.RawFormat;
            Bitmap SaveImage = new Bitmap(targetWidth, targetHeight);
            Graphics g = Graphics.FromImage(SaveImage);
            g.Clear(Color.White);

            //计算缩放图片的大小
            var height = targetHeight;
            var width = targetWidth;
            g.DrawImage(sourceImage, 0, 0, width, height); //在指定坐标处画指定大小的图片
            sourceImage.Dispose();
            return SaveImage;
        }
        catch (Exception ex)  {
            Console.WriteLine("ZoomPicture failed: " + ex.Message);
        }
        return null;
    }

    public static void ZoomPicture(string path, float scale) {
        var input = new Mat(path, ImreadModes.AnyColor | ImreadModes.AnyDepth);
        if (input.Empty()) {
            return;
        }

        var scaledMat = input.Resize(new Size(), scale, scale, InterpolationFlags.Linear);
        if (!scaledMat.Empty()) {
            scaledMat.SaveImage(path);
        }
    }

    public static void ZoomPictureAlongWidth(string path, int targetWidth) {
        var input = new Mat(path, ImreadModes.AnyColor | ImreadModes.AnyDepth);
        if (input.Empty()) {
            return;
        }

        var scale = targetWidth * 1.0f / input.Cols;
        var scaledMat = input.Resize(new Size(), scale, scale, InterpolationFlags.Linear);
        if (!scaledMat.Empty()) {
            scaledMat.SaveImage(path);
        }
    }
}