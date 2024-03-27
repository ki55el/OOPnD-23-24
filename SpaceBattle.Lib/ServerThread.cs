using System.Collections.Concurrent;
using Hwdtech;

namespace SpaceBattle.Lib;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue;
    private Action _behavior;
    private readonly Thread _thread;
    private bool _stop = false;
    private Action _before = () => { };
    private Action _after = () => { };
    public ServerThread(BlockingCollection<ICommand> queue)
    {
        _queue = queue;
        _behavior = () =>
        {
            var cmd = _queue.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                IoC.Resolve<ICommand>("Exception.Handler", ex, cmd).Execute();
            }
        };
        _thread = new Thread(() =>
            {
                _before();
                while (!_stop)
                {
                    _behavior();
                }

                _after();
            }
        );
    }

    public void Start()
    {
        _thread.Start();
    }

    internal void Stop()
    {
        _stop = true;
    }

    internal void SetBehavior(Action behavior)
    {
        _behavior = behavior;
    }

    public void Wait(int ms = 1000)
    {
        _thread.Join(ms);
    }

    public void SetBefore(Action before)
    {
        _before = before;
    }

    public void SetAfter(Action after)
    {
        _after = after;
    }

    internal BlockingCollection<ICommand> GetQueue()
    {
        return _queue;
    }

    public override bool Equals(object? obj)
    {
        return _thread.Equals(obj);
    }

    public override int GetHashCode()
    {
        return _thread.GetHashCode();
    }
}
