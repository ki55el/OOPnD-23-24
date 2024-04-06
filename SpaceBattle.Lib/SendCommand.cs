using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class SendCommand : ICommand
{
    private readonly BlockingCollection<ICommand> _queue;
    private readonly ICommand _cmd;
    public SendCommand(BlockingCollection<ICommand> queue, ICommand cmd)
    {
        _queue = queue;
        _cmd = cmd;
    }
    public void Execute()
    {
        _queue.Add(_cmd);
    }
}
