using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class RegisterHandlerCommandTest
{
    public RegisterHandlerCommandTest()
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
            "Operation.GetHashCode", (object[] args) =>
            {
                return new GetHashCodeStrategy().Build(args[0]);
            }
        ).Execute();

        var sMock = new Mock<IStrategy>();
        sMock.Setup(x => x.Build()).Returns(new Dictionary<object, IHandler>());
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.ExceptionHandler.Tree",
            (object[] args) => sMock.Object.Build(args)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.ExceptionHandler.Register",
            (object[] args) => new RegisterHandlerCommand(
                (IEnumerable<Type>)args[0],
                (IHandler)args[1]
            )
        ).Execute();
    }

    [Fact]
    public void RegisterHandlerCommandTestPositive()
    {
        IoC.Resolve<ICommand>(
            "Game.ExceptionHandler.Register",
            new List<Type> {
                typeof(ICommand),
                typeof(Exception)
            },
            new Mock<IHandler>().Object
        ).Execute();

        IoC.Resolve<ICommand>(
            "Game.ExceptionHandler.Register",
            new List<Type> {
                typeof(IStrategy),
                typeof(InvalidProgramException)
            },
            new Mock<IHandler>().Object
        ).Execute();

        IoC.Resolve<ICommand>(
            "Game.ExceptionHandler.Register",
            new List<Type> {
                typeof(IMovable),
                typeof(DriveNotFoundException)
            },
            new Mock<IHandler>().Object
        ).Execute();

        var handlerTree = IoC.Resolve<IDictionary<object, IHandler>>("Game.ExceptionHandler.Tree");

        Assert.Equal(3, handlerTree.Count());
    }
}
