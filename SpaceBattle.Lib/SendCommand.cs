using System.Collections.Concurrent;
using Hwdtech;

namespace SpaceBattle.Lib;

public class SendCommand : ICommand
{
    private readonly int _id;
    private readonly ICommand _cmd;
    public SendCommand(int id, ICommand cmd)
    {
        _id = id;
        _cmd = cmd;
    }
    public void Execute()
    {
        IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<ICommand>>>("Queue List")[_id].Add(_cmd);
    }
}
