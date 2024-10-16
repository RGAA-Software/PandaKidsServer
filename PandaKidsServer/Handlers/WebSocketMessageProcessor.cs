namespace PandaKidsServer.Handlers;

public class WebSocketMessageProcessor
{

    private readonly AppContext _appContext;
    
    public WebSocketMessageProcessor(AppContext ctx)
    {
        _appContext = ctx;
    }

    public void ProcessBinaryMessage(String msg)
    {
        if (msg == "ping")
        {
            var id = "123456";
            var userMgr = _appContext.GetOnlineUserManager();
            var user = userMgr.FindUserById(id);
            user!.Notify("pong");
        }
    }

    public void ProcessTextMessage(String msg)
    {
        if (msg == "ping")
        {
            var id = "123456";
            var userMgr = _appContext.GetOnlineUserManager();
            var user = userMgr.FindUserById(id);
            user!.Notify("pong");
        }
    }
}