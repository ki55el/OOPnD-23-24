namespace SpaceBattle.Lib;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _st;
    public HardStopCommand(ServerThread st)
    {
        _st = st;
    }
    public void Execute()
    {
        if (!_st.Equals(Thread.CurrentThread))
        {
            throw new Exception("Wrong Thread");
        }

        _st.Stop();
    }
}
