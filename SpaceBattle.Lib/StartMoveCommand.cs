using Hwdtech;

namespace SpaceBattle.Lib;

public class StartMoveCommand : ICommand
{
    private readonly IMoveCommandStartable _order;
    public StartMoveCommand(IMoveCommandStartable order)
    {
        _order = order;
    }
    public void Execute()
    {
        _order.InitialValues.ToList().ForEach(p => _order.Target.SetProperty(p.Key, p.Value));
        var operation = IoC.Resolve<ICommand>("Game.Operation.Move", _order.Target);
        _order.Target.SetProperty("Move", operation);
        IoC.Resolve<IQueue>("Game.Queue").Add(operation);
    }
}
