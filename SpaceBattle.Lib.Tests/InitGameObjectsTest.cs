using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class InitGameObjectsTest
{
    public InitGameObjectsTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SetProperty",
            (object[] args) =>
            {
                var obj = (IUObject)args[0];
                var key = (string)args[1];
                var value = args[2];

                return new ActionCommand(() =>
                {
                    obj.SetProperty(key, value);
                });
            }
        ).Execute();

        var mockObj = new Mock<IUObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create new GameObject",
            (object[] args) => mockObj.Object
        ).Execute();

        var objDict = new ConcurrentDictionary<string, IUObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Register GameObject",
            (object[] args) =>
            {
                var obj = (IUObject)args[0];
                var id = (string)args[1];

                return new ActionCommand(() =>
                {
                    objDict[id] = obj;
                });
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameObject List",
            (object[] args) => objDict
        ).Execute();
    }

    [Fact]
    public void SetFuelTest()
    {
        var mockObj = new Mock<IUObject>();
        mockObj.Setup(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

        var obj = mockObj.Object;
        var fuel = 1_000;
        new SetFuelCommand(obj, fuel).Execute();

        mockObj.Verify(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void SetPositionTest()
    {
        var mockObj = new Mock<IUObject>();
        mockObj.Setup(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

        var posList = new List<int[]>
        {
            new int[]{0,0},
            new int[]{0,1},
            new int[]{1,0},
            new int[]{1,1},
        };

        var objList = Enumerable.Range(0, posList.Count).Select(_ => mockObj.Object).ToList();

        new SetAllPositionsCommand(objList, posList).Execute();

        mockObj.Verify(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>()), Times.AtLeast(posList.Count));
    }

    [Fact]
    public void CreateGameObjectsTest()
    {
        var count = 5;
        new CreateGameObjectsCommand(count).Execute();

        var dict = IoC.Resolve<ConcurrentDictionary<string, IUObject>>("GameObject List");
        Assert.Equal(count, dict.Count);
    }
}
