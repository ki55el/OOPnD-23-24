using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class StartMoveCommandTest
{
    private readonly Mock<IMoveCommandStartable> _moveCommandStartableMock;
    private readonly Mock<IUObject> _uObjectMock;
    private readonly StartMoveCommand _startMoveCommand;

    public StartMoveCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _moveCommandStartableMock = new Mock<IMoveCommandStartable>();
        _uObjectMock = new Mock<IUObject>();

        _moveCommandStartableMock.Setup(mcs => mcs.Target).Returns(_uObjectMock.Object);
        _moveCommandStartableMock.Setup(mcs => mcs.InitialValues).Returns(new Dictionary<string, object>());

        _startMoveCommand = new StartMoveCommand(_moveCommandStartableMock.Object);
    }

    [Fact]
    public void LongOperationTest()
    {
        var cmd = new Mock<ICommand>();
        var qMock = new Mock<IQueue>();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Operation.Move",
            (object[] args) =>
            {
                return cmd.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Queue",
            (object[] args) =>
            {
                return qMock.Object;
            }
        ).Execute();

        _startMoveCommand.Execute();

        _moveCommandStartableMock.Verify(m => m.InitialValues, Times.Once());
        qMock.Verify(q => q.Add(It.IsAny<ICommand>()), Times.Once());
    }
}
