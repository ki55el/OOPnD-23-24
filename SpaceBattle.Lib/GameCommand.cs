using System.Diagnostics;
using Hwdtech;

namespace SpaceBattle.Lib;

public class GameCommand : ICommand
{
    private readonly object _scope;
    private readonly Queue<ICommand> _q;
    public GameCommand(object scope, Queue<ICommand> q)
    {
        _scope = scope;
        _q = q;
    }
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", _scope).Execute();
        var timeQuant = IoC.Resolve<int>("Game.TimeQuant");
        var stopwatch = new Stopwatch();
        while (_q.Count > 0 && timeQuant > 0)
        {
            stopwatch.Restart();
            var cmd = _q.Dequeue();
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                IoC.Resolve<ICommand>("Exception.Handler", cmd, ex).Execute();
            }

            stopwatch.Stop();
            timeQuant -= (int)stopwatch.ElapsedMilliseconds;
        }
    }
}
