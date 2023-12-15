using Moq;
using SpaceBattle.Lib;
using TechTalk.SpecFlow;

namespace SpaceBattleTests;

[Binding]
public class Move
{
    private readonly Mock<IMovable> _movable;

    private Action commandExecutionLambda;

    public Move()
    {
        _movable = new Mock<IMovable>();

        commandExecutionLambda = () => { };

    }
    [Given(@"космический корабль находится в точке пространства с координатами \((.*), (.*)\)")]
    public void ДаноКосмическийКорабльНаходитсяВТочкеПространстваСКоординатами(int p0, int p1)
    {
        _movable.SetupGet(m => m.Position).Returns(new Vector(new int[] { p0, p1 }));
    }

    [Given(@"космический корабль, положение в пространстве которого невозможно определить")]
    public void ДаноКосмическийКорабльПоложениеВПространствеКоторогоНевозможноОпределить()
    {
        _movable.SetupGet(m => m.Position).Throws<Exception>();
    }

    [Given(@"имеет мгновенную скорость \((.*), (.*)\)")]
    public void ДаноИмеетМгновеннуюСкорость(int p0, int p1)
    {
        _movable.SetupGet(m => m.Velocity).Returns(new Vector(new int[] { p0, p1 }));
    }

    [Given(@"скорость корабля определить невозможно")]
    public void ДаноСкоростьКорабляОпределитьНевозможно()
    {
        _movable.SetupGet(m => m.Velocity).Throws<Exception>();
    }

    [Given(@"изменить положение в пространстве космического корабля невозможно")]
    public void ДаноИзменитьПоложениеВПространствеКосмическогоКорабляНевозможно()
    {
        _movable.SetupGet(m => m.Velocity).Throws<Exception>();
    }

    [When("происходит прямолинейное равномерное движение без деформации")]
    public void КогдаПроисходитПрямолинейноеРавномерноеДвижениеБезДеформации()
    {
        var mc = new MoveCommand(_movable.Object);
        commandExecutionLambda = () => mc.Execute();
    }

    [Then(@"космический корабль перемещается в точку пространства с координатами \((.*), (.*)\)")]
    public void ТогдаКосмическийКорабльПеремещаетсяВТочкуПространстваСКоординатами(int p0, int p1)
    {
        commandExecutionLambda();
        _movable.VerifySet(m => m.Position = It.Is<Vector>(p => p.nums[0] == p0 && p.nums[1] == p1));
    }

    [Then(@"возникает ошибка Exception")]
    public void ТогдаВозникаетОшибкаException()
    {
        Assert.Throws<Exception>(() => commandExecutionLambda());
    }
}
