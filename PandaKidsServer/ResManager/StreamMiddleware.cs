using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PandaKidsServer.ResManager;

public class OuterImgMiddleware(RequestDelegate next, IHostingEnvironment env)
{
    public static string RootPath { get; set; }

    public async Task Invoke(HttpContext context) {
        var path = context.Request.Path.ToString();
        var prefix = "/pandakids/stream";
        if (context.Request.Method == "GET" && !string.IsNullOrEmpty(path)) {
            if (path.Contains(prefix)) {
                path = path.Substring(prefix.Length);
                var s = RootPath + path;
                var file = new FileInfo(s);
                if (file.Exists) {
                    var length = path.LastIndexOf(".") - path.LastIndexOf("/") - 1;
                    Console.WriteLine("file length: " + length + ", file length: " + file.Length);
                    context.Response.Headers["Etag"] = path.Substring(path.LastIndexOf("/") + 1, length);
                    //context.Response.Headers["Last-Modified"] = new DateTime(2024).ToString("r");
                    context.Response.Headers["Accept-Ranges"] = "bytes";
                    // context.Response.Headers["Content-Length"] = file.Length.ToString();
                    if (path.EndsWith(".mp4")) {
                        context.Response.ContentType = "video/mp4";
                        //分段读取内容
                        var download = new StreamRange(context);
                        download.WriteFile(s);
                    }
                    else {
                        context.Response.ContentType = "image/jpeg";
                        context.Response.Headers["Cache-Control"] = "public"; //指定客户端，服务器都处理缓存
                        await context.Response.SendFileAsync(s);
                    }
                }

                return;
            }
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