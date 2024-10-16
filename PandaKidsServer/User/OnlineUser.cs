using Newtonsoft.Json;
using PandaKidsServer.Handlers;

namespace PandaKidsServer.User;

public class OnlineUser
{
    private readonly AppContext _appContext;
    private readonly WebSocketHandler _wsHandler;

    [JsonProperty("id")]
    public string Id = "";

    public static OnlineUser Make(AppContext ctx, WebSocketHandler handler, string id)
    {
        return new OnlineUser(ctx, handler)
        {
            Id = id
        };
    }

    public OnlineUser(AppContext ctx, WebSocketHandler handler)
    {
        _appContext = ctx;
        _wsHandler = handler;
    }

    public void Notify(String msg)
    {
        _wsHandler.SendMessage(msg);
    }

}