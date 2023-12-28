using Hwdtech;

namespace SpaceBattle.Lib;

public class BuildTreeCommand : ICommand
{
    private readonly ITreeBuildable _build;

    public BuildTreeCommand(ITreeBuildable build)
    {
        _build = build;
    }

    public void Execute()
    {
        _build.Tree().ForEach(nums =>
        {
            var node = IoC.Resolve<Dictionary<int, object>>("Game.BuildDecisionTree");
            nums.ForEach(num =>
            {
                node.TryAdd(num, new Dictionary<int, object>());
                node = (Dictionary<int, object>)node[num];
            });
        });
    }
}
