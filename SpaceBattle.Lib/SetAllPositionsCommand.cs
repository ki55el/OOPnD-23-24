using Hwdtech;

namespace SpaceBattle.Lib;

public class SetAllPositionsCommand : ICommand
{
    private readonly List<IUObject> _objList;
    private readonly List<int[]> _positionList;
    public SetAllPositionsCommand(List<IUObject> objList, List<int[]> positionList)
    {
        _objList = objList;
        _positionList = positionList;
    }
    public void Execute()
    {
        var seq = _objList.Zip(_positionList).ToList();

        seq.ForEach(p =>
        {
            var obj = p.First;
            var position = p.Second;

            IoC.Resolve<ICommand>("SetProperty", obj, "Position", position).Execute();
        });
    }
}
