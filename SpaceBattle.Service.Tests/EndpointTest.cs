using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Service.Tests;

public class EndpointTest
{
    public EndpointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        var qDict = new ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Queue List",
            (object[] args) => qDict
        ).Execute();

        var cmdMock = new Mock<Lib.ICommand>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create Command",
            (object[] args) => cmdMock.Object
        ).Execute();
    }

    [Fact]
    public void EndpointAcceptedMessage()
    {
        var msg = new MessageContract()
        {
            Type = "start movement",
            GameID = 548,
            GameItemID = "asdfg",
            Properties = new Dictionary<string, object>() { { "initial velocity", 2 } }
        };

        var qDict = IoC.Resolve<ConcurrentDictionary<int, BlockingCollection<Lib.ICommand>>>("Queue List");
        qDict.TryAdd(548, new BlockingCollection<Lib.ICommand>());

        var statuscode = new Endpoint().POST(msg);
        Assert.Equal(202, statuscode);
    }

    [Fact]
    public void EndpointBadRequestedMessage()
    {
        var msg = new MessageContract()
        {
            Type = "start movement",
            GameID = 548,
            GameItemID = "asdfg",
            Properties = new Dictionary<string, object>() { { "initial velocity", 2 } }
        };

        var statuscode = new Endpoint().POST(msg);
        Assert.Equal(400, statuscode);
    }
}
