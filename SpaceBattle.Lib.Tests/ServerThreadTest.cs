using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ServerThreadTest
{
    public Random rnd = new();

    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        var qDict = new ConcurrentDictionary<int, BlockingCollection<ICommand>>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Queue List",
            (object[] args) => qDict
        ).Execute();

        var stDict = new ConcurrentDictionary<int, ServerThread>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThread List",
            (object[] args) => stDict
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create And Start Thread",
            (object[] args) =>
            {
                var id = (int)args[0];
                var act = () => { };

                if (args.Length == 2 && args[1].GetType() == typeof(Action))
                {
                    act = (Action)args[1];
                }

                return new ActionCommand(() =>
                {
                    new CreateAndStartThreadCommand(id).Execute();
                    new ActionCommand(act).Execute();
                });
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command",
            (object[] args) =>
            {
                var id = (int)args[0];
                var cmd = (ICommand)args[1];

                return new ActionCommand(() =>
                {
                    new SendCommand(id, cmd).Execute();
                });
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Hard Stop The Thread",
            (object[] args) =>
            {
                var id = (int)args[0];
                var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread List")[id];
                var act = () => { };

                if (args.Length == 2 && args[1].GetType() == typeof(Action))
                {
                    act = (Action)args[1];
                }

                return new ActionCommand(() =>
                {
                    new HardStopCommand(st).Execute();
                    new ActionCommand(act).Execute();
                });
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Soft Stop The Thread",
            (object[] args) =>
            {
                var id = (int)args[0];
                var st = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread List")[id];
                var act = () => { };

                if (args.Length == 2 && args[1].GetType() == typeof(Action))
                {
                    act = (Action)args[1];
                }

                return new ActionCommand(() =>
                {
                    new SoftStopCommand(st).Execute();
                    new ActionCommand(act).Execute();
                });
            }
        ).Execute();
    }

    [Fact]
    public void HardStopCommandShouldStopServerThread()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Current")
            )
        ).Execute();

        var id = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id
        ).Execute();

        var mre = new ManualResetEvent(false);
        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread",
            id,
            () => { mre.Set(); }
        );

        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();

        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<ICommand>("Send Command", id, hs).Execute();
        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();

        mre.WaitOne();

        cmd.Verify(x => x.Execute(), Times.Once);

        var queue = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<ICommand>>>("Queue List")[id];
        Assert.Single(queue);
    }

    [Fact]
    public void HardStopCommandWrongThreadException()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Current")
            )
        ).Execute();

        var id = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id
        ).Execute();

        var mre = new ManualResetEvent(false);
        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread",
            id,
            () => { mre.Set(); }
        );

        IoC.Resolve<ICommand>("Send Command", id, hs).Execute();

        mre.WaitOne();

        Assert.Throws<Exception>(hs.Execute);

        var queue = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<ICommand>>>("Queue List")[id];
        Assert.Empty(queue);
    }

    [Fact]
    public void SoftStopCommandShouldStopServerThread()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Current")
            )
        ).Execute();

        var id = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id
        ).Execute();

        var mre = new ManualResetEvent(false);
        var ss = IoC.Resolve<ICommand>("Soft Stop The Thread",
            id,
            () => { mre.Set(); }
        );

        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();

        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<ICommand>("Send Command", id, ss).Execute();
        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();

        mre.WaitOne();

        cmd.Verify(x => x.Execute(), Times.AtLeast(2));

        var queue = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<ICommand>>>("Queue List")[id];
        Assert.Empty(queue);
    }

    [Fact]
    public void SoftStopCommandHandlerException()
    {
        var scope = new ICommandAdapter(
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New",
                    IoC.Resolve<object>("Scopes.Current")
                )
            )
        );

        var handlerEx = new ICommandAdapter(
            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Exception.Handler",
                (object[] args) =>
                {
                    return new Mock<ICommand>().Object;
                }
            )
        );

        var id = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id
        ).Execute();

        var mre = new ManualResetEvent(false);
        var ss = IoC.Resolve<ICommand>("Soft Stop The Thread",
            id,
            () => { mre.Set(); }
        );

        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();

        var cmdEx = new Mock<ICommand>();
        cmdEx.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<ICommand>("Send Command", id, scope).Execute();
        IoC.Resolve<ICommand>("Send Command", id, handlerEx).Execute();
        IoC.Resolve<ICommand>("Send Command", id, cmdEx.Object).Execute();
        IoC.Resolve<ICommand>("Send Command", id, ss).Execute();
        IoC.Resolve<ICommand>("Send Command", id, cmdEx.Object).Execute();

        mre.WaitOne();

        Assert.Throws<Exception>(() => ss.Execute());

        var queue = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<ICommand>>>("Queue List")[id];
        Assert.Empty(queue);
    }

    [Fact]
    public void SoftStopCommandWrongThreadException()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Current")
            )
        ).Execute();

        var id = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id
        ).Execute();

        var mre = new ManualResetEvent(false);
        var ss = IoC.Resolve<ICommand>("Soft Stop The Thread",
            id,
            () => { mre.Set(); }
        );

        IoC.Resolve<ICommand>("Send Command", id, ss).Execute();

        mre.WaitOne();

        Assert.Throws<Exception>(ss.Execute);

        var queue = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<ICommand>>>("Queue List")[id];
        Assert.Empty(queue);
    }

    [Fact]
    public void TestGetHashCode()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Current")
            )
        ).Execute();

        var id1 = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id1
        ).Execute();
        var st1 = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread List")[id1];

        var id2 = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id2
        ).Execute();
        var st2 = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread List")[id2];

        Assert.True(st1.GetHashCode() != st2.GetHashCode());
    }

    [Fact]
    public void TestEquals()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Current")
            )
        ).Execute();

        var id1 = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id1
        ).Execute();
        var st1 = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread List")[id1];

        var id2 = rnd.Next();
        IoC.Resolve<ICommand>("Create And Start Thread",
            id2
        ).Execute();
        var st2 = IoC.Resolve<ConcurrentDictionary<int, ServerThread>>("ServerThread List")[id2];

        Assert.False(st1.Equals(st2));
    }
}
