using System.Net.WebSockets;
using System.Text;

namespace PandaKidsServer.Handlers;

public class WebSocketHandler
{
    private readonly AppContext _appContext;
    private readonly WebSocketMessageProcessor _msgProcessor;
    private int _active = -1;
    private WebSocket? _webSocket;

    public WebSocketHandler(AppContext ctx) {
        _appContext = ctx;
        _msgProcessor = new WebSocketMessageProcessor(ctx);
    }

    public async Task Handle(WebSocket ws, string id) {
        _webSocket = ws;
        try {
            Interlocked.Exchange(ref _active, 0);
            var sb = new StringBuilder();
            while (_webSocket.State == WebSocketState.Open) {
                sb.Clear();
                while (true) {
                    var isBinaryMessage = false;
                    var isTextMessage = false;
                    var buffer = new ArraySegment<byte>(new byte[4096]);
                    var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Binary && buffer.Array != null) {
                        isBinaryMessage = true;
                        var msg = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        sb.Append(msg);
                    }
                    else if (result.MessageType == WebSocketMessageType.Text && buffer.Array != null) {
                        isTextMessage = true;
                        var msg = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        sb.Append(msg);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close) {
                        Console.WriteLine("Close WS");
                    }

                    if (result.EndOfMessage) {
                        var msg = sb.ToString();
                        if (isBinaryMessage)
                            _msgProcessor.ProcessBinaryMessage(msg);
                        else if (isTextMessage) _msgProcessor.ProcessTextMessage(msg);
                        break;
                    }
                }
            }
        }
        catch (Exception e) {
            Console.Write("WS error: " + e.Message);
        }

        _appContext.GetOnlineUserManager().RemoveUser(id);
        Interlocked.Exchange(ref _active, -1);
    }

    private bool IsActive() {
        return _webSocket is { State: WebSocketState.Open } && Thread.VolatileRead(ref _active) != -1;
    }

    public async void SendMessage(string msg) {
        if (_webSocket == null || !IsActive()) return;
        var byteArray = Encoding.UTF8.GetBytes(msg);
        var byteSegment = new ArraySegment<byte>(byteArray);
        await _webSocket.SendAsync(byteSegment, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}