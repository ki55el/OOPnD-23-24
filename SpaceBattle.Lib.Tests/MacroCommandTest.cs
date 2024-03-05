using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class MacroCommandTest
{
    public MacroCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>(
                "Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.MacroCommand.Create",
            (object[] args) =>
            {
                var commands = (IEnumerable<ICommand>)args[0];
                return new MacroCommand(commands);
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.MacroCommand.Strategy",
            (object[] args) =>
            {
                var name = (string)args[0];
                var obj = (IUObject)args[1];
                return new MacroCommandStrategy().Build(name, obj);
            }
        ).Execute();
    }

    [Fact]
    public void MacroCommandPositive()
    {
        var name = "MoveAndRotate";
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Operation." + name,
            (object[] args) =>
                new string[] { "Game.Operation.Move", "Game.Operation.Turn" }
        ).Execute();

        var mc = new Mock<ICommand>();
        mc.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Operation.Move",
            (object[] args) => mc.Object
        ).Execute();

        var tc = new Mock<ICommand>();
        tc.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Operation.Turn",
            (object[] args) => tc.Object
        ).Execute();

        var obj = new Mock<IUObject>();
        IoC.Resolve<ICommand>(
            "Game.MacroCommand.Strategy",
            name,
            obj.Object
        ).Execute();

        mc.Verify(x => x.Execute(), Times.Once);
        tc.Verify(x => x.Execute(), Times.Once);
    }
}
