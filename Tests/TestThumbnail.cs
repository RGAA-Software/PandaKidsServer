using System.Text;
using System.Text.Unicode;
using NUnit.Framework.Internal;
using PandaKidsServer.Common;

namespace Tests;

public class TestThumbnail
{
    [SetUp]
    public void Setup() {
        
    }

    [Test]
    public void GenerateThumbnail() {
        // FFmpegHelper.GetPicFromVideo("D:\\PandaKidsResources\\Resources\\Preset\\英文版1-8季（英文字幕）\\06、第六季 25集\\Peppa Pig S06E02 Chinese New Year.mp4",
        //     "320*320", "111222");

        var path = "D:\\PandaKidsResources\\Resources\\Preset\\英文版1-8季（英文字幕）\\06、第六季 25集\\Peppa Pig S06E02 Chinese New Year.mp4";
        var thumbnailPath = FFmpegHelper.GenerateThumbnail(path);
        Assert.That(thumbnailPath, Is.Not.EqualTo(null));
        Console.WriteLine("output thumbnail: " + thumbnailPath);
    }
}