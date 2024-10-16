using PandaKidsServer.User;

namespace PandaKidsServer;

public class AppContext
{
    private readonly OnlineUserManager _onlineUserManager;

    public AppContext()
    {
        _onlineUserManager = new OnlineUserManager(this);
    }

    public OnlineUserManager GetOnlineUserManager()
    {
        return _onlineUserManager;
    }
}