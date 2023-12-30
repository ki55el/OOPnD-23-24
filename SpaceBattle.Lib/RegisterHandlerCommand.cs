using Hwdtech;

namespace SpaceBattle.Lib;

public class RegisterHandlerCommand : ICommand
{
    private readonly IEnumerable<Type> _types;
    private readonly IHandler _handler;
    public RegisterHandlerCommand(IEnumerable<Type> types, IHandler handler)
    {
        _types = types;
        _handler = handler;
    }
    public void Execute()
    {
        IoC.Resolve<IDictionary<object, IHandler>>(
            "Game.ExceptionHandler.Tree"
        ).TryAdd(
            IoC.Resolve<int>(
                "Operation.GetHashCode",
                _types
            ),
            _handler
        );
    }
}
