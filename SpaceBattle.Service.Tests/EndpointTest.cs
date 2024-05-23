using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Lib;

namespace SpaceBattle.Service.Tests;

public class EndpointTest
{
    public Mock<Lib.ICommand> exHandler = new();
    public EndpointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        var idDict = new ConcurrentDictionary<string, string>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ID List",
            (object[] args) => idDict
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get Thread ID by Game ID",
            (object[] args) =>
            {
                var game_id = (string)args[0];
                var thread_id = IoC.Resolve<ConcurrentDictionary<string, string>>("ID List")[game_id];

                return thread_id;
            }
        ).Execute();

        var cmdMock = new Mock<Lib.ICommand>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create Command",
            (object[] args) => cmdMock.Object
        ).Execute();

        var qDict = new ConcurrentDictionary<string, BlockingCollection<Lib.ICommand>>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Queue List",
            (object[] args) => qDict
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command",
            (object[] args) =>
            {
                var id = (string)args[0];
                var cmd = (Lib.ICommand)args[1];
                var queue = IoC.Resolve<ConcurrentDictionary<string, BlockingCollection<Lib.ICommand>>>("Queue List")[id];

                return new ActionCommand(() =>
                {
                    new SendCommand(queue, cmd).Execute();
                });
            }
        ).Execute();

        exHandler.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HttpController.ExceptionHandler",
            (object[] args) => exHandler.Object
        ).Execute();
    }

    [Fact]
    public void EndpointAcceptedMessage()
    {
        var msg = new MessageContract()
        {
            Type = "start movement",
            GameID = "asdfg",
            GameItemID = 548,
            Properties = new Dictionary<string, object>() { { "initial velocity", 2 } }
        };

        var idDict = IoC.Resolve<ConcurrentDictionary<string, string>>("ID List");
        var id = Guid.NewGuid().ToString();
        idDict.TryAdd(msg.GameID, id);

        var qDict = IoC.Resolve<ConcurrentDictionary<string, BlockingCollection<Lib.ICommand>>>("Queue List");
        qDict.TryAdd(id, new BlockingCollection<Lib.ICommand>());
        var queue = qDict[id];

        new Endpoint().POST(msg);
        exHandler.Verify(x => x.Execute(), Times.Never);
        Assert.Single(queue);
    }

    [Fact]
    public void EndpointBadRequestedMessage()
    {
        var msg = new MessageContract()
        {
            Type = "start movement",
            GameID = "asdfg",
            GameItemID = 548,
        };

        var id = Guid.NewGuid().ToString();

        var qDict = IoC.Resolve<ConcurrentDictionary<string, BlockingCollection<Lib.ICommand>>>("Queue List");
        qDict.TryAdd(id, new BlockingCollection<Lib.ICommand>());
        var queue = qDict[id];

        new Endpoint().POST(msg);
        exHandler.Verify(x => x.Execute(), Times.Once);
        Assert.Empty(queue);
    }
}
