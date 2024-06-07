using Hwdtech;

namespace SpaceBattle.Lib;

public class SetFuelCommand : ICommand
{
    private readonly IUObject _obj;
    private readonly int _fuel;
    public SetFuelCommand(IUObject obj, int fuel)
    {
        _obj = obj;
        _fuel = fuel;
    }
    public void Execute()
    {
        IoC.Resolve<ICommand>("SetProperty", _obj, "Fuel", _fuel).Execute();
    }
}
