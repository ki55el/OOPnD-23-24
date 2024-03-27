using Hwdtech;

namespace SpaceBattle.Lib;

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _st;
    public SoftStopCommand(ServerThread st)
    {
        _st = st;
    }
    public void Execute()
    {
        if (!_st.Equals(Thread.CurrentThread))
        {
            throw new Exception("Wrong Thread");
        }

        _st.SetBehavior(() =>
        {
            var queue = _st.GetQueue();
            if (queue.Count > 0)
            {
                var cmd = queue.Take();
                try
                {
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    IoC.Resolve<ICommand>("Exception.Handler", ex, cmd).Execute();
                }
            }
            else
            {
                _st.Stop();
            }
        });
    }
}
