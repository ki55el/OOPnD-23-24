using System.Collections.Concurrent;
using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateAndStartThreadCommand : ICommand
{
    private readonly int _id;
    public CreateAndStartThreadCommand(int id)
    {
        _id = id;
    }
    public void Execute()
    {
        var queue = new BlockingCollection<ICommand>(10);
        IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<ICommand>>>("Queue List").TryAdd(_id, queue);

        var st = new ServerThread(queue);
        IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread List").TryAdd(_id, st);

        st.Start();
    }
}
