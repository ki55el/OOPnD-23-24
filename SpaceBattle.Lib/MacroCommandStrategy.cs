using Hwdtech;

namespace SpaceBattle.Lib;

public class MacroCommandStrategy : IStrategy
{
    public object Build(params object[] args)
    {
        var name = (string)args[0];
        var obj = (IUObject)args[1];

        var commands = IoC.Resolve<string[]>("Operation." + name).Select(
            p => IoC.Resolve<ICommand>(p, obj));

        return IoC.Resolve<ICommand>("Game.MacroCommand.Create", commands);
    }
}
