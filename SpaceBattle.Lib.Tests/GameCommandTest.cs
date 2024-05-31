using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class GameCommandTest
{
    public GameCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();
    }

    [Fact]
    public void GameCommandExecuted()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            scope
        ).Execute();

        var timeQuant = 100_000;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuant",
            (object[] args) =>
            {
                return (object)timeQuant;
            }
        ).Execute();

        var mre = new ManualResetEvent(false);
        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(new ActionCommand(() => { mre.Set(); }));

        new GameCommand(scope, q).Execute();

        mre.WaitOne();
        cmd.Verify(x => x.Execute(), Times.AtLeast(2));
        Assert.Empty(q);
    }

    [Fact]
    public void GameCommandThrewException()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            scope
        ).Execute();

        var timeQuant = 100_000;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuant",
            (object[] args) =>
            {
                return (object)timeQuant;
            }
        ).Execute();

        var exHandler = new Mock<ICommand>();
        exHandler.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handler",
            (object[] args) => exHandler.Object
        ).Execute();

        var mre = new ManualResetEvent(false);

        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();

        var cmdEx = new Mock<ICommand>();
        cmdEx.Setup(x => x.Execute()).Throws(new Exception());

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmdEx.Object);
        q.Enqueue(new ActionCommand(() => { mre.Set(); }));

        new GameCommand(scope, q).Execute();

        mre.WaitOne();
        exHandler.Verify(x => x.Execute(), Times.Once);
        Assert.Empty(q);
    }

    [Fact]
    public void GameCommandWithNoTime()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            scope
        ).Execute();

        var timeQuant = 0;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuant",
            (object[] args) =>
            {
                return (object)timeQuant;
            }
        ).Execute();

        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);

        new GameCommand(scope, q).Execute();

        cmd.Verify(x => x.Execute(), Times.Never);
        Assert.Single(q);
    }
}
