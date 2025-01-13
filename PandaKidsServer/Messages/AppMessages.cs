using MediatR;

namespace PandaKidsServer.Messages;

public abstract class AppMessages
{
    public class MsgTimer5S(AppContext ctx) : INotification
    {
        public AppContext AppCtx = ctx;
    }
}