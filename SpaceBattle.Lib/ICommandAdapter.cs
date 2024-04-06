namespace SpaceBattle.Lib;

public class ICommandAdapter : ICommand
{
    private readonly Hwdtech.ICommand _cmd;

    public ICommandAdapter(Hwdtech.ICommand cmd)
    {
        _cmd = cmd;
    }
    public void Execute()
    {
        _cmd.Execute();
    }
}
