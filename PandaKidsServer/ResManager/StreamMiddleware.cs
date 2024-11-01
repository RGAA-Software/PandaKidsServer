using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PandaKidsServer.ResManager;

public class OuterImgMiddleware(RequestDelegate next, IHostingEnvironment env)
{
    public static string RootPath { get; set; }

    public async Task Invoke(HttpContext context) {
        var path = context.Request.Path.ToString();
        var prefix = "/pandakids/stream";
        if (context.Request.Method == "GET" && !string.IsNullOrEmpty(path) && path.Contains(prefix)) {
            var url = context.Request.GetDisplayUrl();
            var decodedUrl = HttpUtility.UrlDecode(url);
            
            var uri = new Uri(decodedUrl);
            var decodedPath = uri.LocalPath;
            decodedPath = decodedPath[prefix.Length..];
            var displayFilePath = RootPath + decodedPath;
            
            var file = new FileInfo(displayFilePath);
            if (!file.Exists) {
                Console.WriteLine("File not exists:" + displayFilePath);
                return;
            }
            
            var length = path.LastIndexOf('.') - path.LastIndexOf('/') - 1;
            context.Response.Headers["Etag"] = path.Substring(path.LastIndexOf('/') + 1, length);
            context.Response.Headers["Accept-Ranges"] = "bytes";
            // context.Response.Headers["Content-Length"] = file.Length.ToString();
            if (path.EndsWith(".mp4")) {
                context.Response.ContentType = "video/mp4";
                var stream = new StreamRange(context);
                stream.WriteFile(displayFilePath);
            }
            else if (path.EndsWith(".mp3")) {
                context.Response.ContentType = "video/mp3";
                var stream = new StreamRange(context);
                stream.WriteFile(displayFilePath);
            }
            else {
                context.Response.ContentType = "image/jpeg";
                context.Response.Headers["Cache-Control"] = "public";
                await context.Response.SendFileAsync(displayFilePath);
            }
            return;
        }

        await next(context);
    }
}

public static class MvcExtensions
{
    public static IApplicationBuilder UseOutImg(this IApplicationBuilder builder) {
        return builder.UseMiddleware<OuterImgMiddleware>();
    }
}