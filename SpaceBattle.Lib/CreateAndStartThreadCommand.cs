using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class CreateAndStartThreadCommand : ICommand
{
    private readonly BlockingCollection<ICommand> _queue;
    private readonly ServerThread _st;
    public CreateAndStartThreadCommand(BlockingCollection<ICommand> queue)
    {
        _queue = queue;
        _st = new ServerThread(_queue);
    }
    public void Execute()
    {
        _st.Start();
    }

    public ServerThread GetServerThread()
    {
        return _st;
    }
}
