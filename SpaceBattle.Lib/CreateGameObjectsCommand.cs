using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateGameObjectsCommand : ICommand
{
    private readonly int _count;
    public CreateGameObjectsCommand(int count)
    {
        _count = count;
    }
    public void Execute()
    {
        var idList = Enumerable.Range(0, _count).Select(_ => Guid.NewGuid().ToString()).ToList();

        idList.ForEach(id =>
        {
            var obj = IoC.Resolve<IUObject>("Create new GameObject");
            IoC.Resolve<ICommand>("Register GameObject", obj, id).Execute();
        });
    }
}
