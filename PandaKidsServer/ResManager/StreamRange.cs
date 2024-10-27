using Serilog;
using static System.Int32;

namespace PandaKidsServer.ResManager;

public class StreamRange(HttpContext ctx)  {
    private const int HttpRangeSize = 1024 * 1024 * 5;
    private readonly HttpRequest _request = ctx.Request;
    private readonly HttpResponse _response = ctx.Response;

    public void WriteFile(string file) {
        using var fs = File.OpenRead(file);
        WriteStream(fs);
    }

    private void WriteStream(Stream fs) {
        foreach (var requestHeader in _request.Headers) {
            Console.WriteLine("key: " + requestHeader.Key + ", val: " + requestHeader.Value);
        }
        string? range = _request.Headers["Range"];

        range = range ?? "";
        range = range.Trim().ToLower();
        if (fs.CanSeek) {
            Console.WriteLine("Range: " + range);
            if (range.StartsWith("bytes=") && range.Contains("-")) {
                var rgs = range.Substring(6).Split('-');
                TryParse(rgs[0], out var start);
                TryParse(rgs[1], out var end);
                if (rgs[0] == "") {
                    start = (int)fs.Length - end;
                    end = (int)fs.Length - 1;
                }

                if (rgs[1] == "") {
                    end = (int)fs.Length - 1;
                }
                WriteRangeStream(fs, start, end);
            }
            else {
                int l;
                var buffer = new byte[40960];
                while ((l = fs.Read(buffer, 0, buffer.Length)) > 0) _response.Body.Write(buffer, 0, l);
            }
        }
    }

    private void WriteRangeStream(Stream fs, int start, int end) {
        Console.WriteLine("range stream: " + start + ", end: " + end);
        var rangLen = end - start + 1;
        if (rangLen > 0) {
            if (rangLen > HttpRangeSize) {
                rangLen = HttpRangeSize;
                end = start + HttpRangeSize - 1;
            }
        }
        else {
            Console.WriteLine("Err range start: " + start + ", end: " + end);
            return;
        }

        var size = fs.Length;
        if (start == 0 && end + 1 >= size) {
            _response.StatusCode = 200;
        }
        else {
            _response.StatusCode = 206;
        }

        // response.Headers.Add("Accept-Ranges", "bytes");
        _response.Headers.Append("Content-Range", $"bytes {start}-{end}/{size}");
        _response.Headers.Append("Content-Length", rangLen.ToString());

        Console.WriteLine($"==> bytes {start}-{end}/{size}");
        Console.WriteLine($"==> bytes " + rangLen.ToString());

        int total = 0;
        var buffer = new byte[40960];
        try {
            fs.Seek(start, SeekOrigin.Begin);
            int readLen;
            while (total < rangLen && (readLen = fs.Read(buffer, 0, buffer.Length)) > 0) {
                total += readLen;
                if (total > rangLen) {
                    readLen -= total - rangLen;
                    total = rangLen;
                }
                _response.Body.Write(buffer, 0, readLen);
            }
        }
        catch (Exception ex) {
            Log.Error("Error: " + ex.Message);
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}