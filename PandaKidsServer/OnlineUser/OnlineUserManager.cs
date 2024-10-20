namespace PandaKidsServer.OnlineUser;

public class OnlineUserManager
{
    private readonly AppContext _appContext;
    private readonly object _userLock = new();
    private readonly Dictionary<string, OnlineUser> _onlineUsers = new();

    public OnlineUserManager(AppContext ctx) {
        _appContext = ctx;
    }

    public void AddUser(OnlineUser user) {
        lock (_userLock) {
            _onlineUsers.Remove(user.Id);
            _onlineUsers.Add(user.Id, user);
        }
    }

    public OnlineUser? RemoveUser(string id) {
        lock (_userLock) {
            _onlineUsers.TryGetValue(id, out var user);
            _onlineUsers.Remove(id);
            return user;
        }
    }

    public OnlineUser? FindUserById(string id) {
        lock (_userLock) {
            _onlineUsers.TryGetValue(id, out var user);
            return user;
        }
    }
}