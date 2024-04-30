using System.Collections.Concurrent;
using Hwdtech;
using SpaceBattle.Lib;

namespace SpaceBattle.Service;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class Endpoint : IEndpoint
{
    public int POST(MessageContract param)
    {
        try
        {
            var q = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>>("Queue List")[param.GameID];
            var cmd = IoC.Resolve<Lib.ICommand>("Create Command", param.Type, param.GameItemID, param.Properties);
            new SendCommand(q, cmd).Execute();

            return 202;
        }
        catch
        {
            return 400;
        }
    }
}
