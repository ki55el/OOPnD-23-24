using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Dict = System.Collections.Generic.Dictionary<int, object>;

namespace SpaceBattle.Lib.Tests;

public class BuildTreeCommandTest
{
    public BuildTreeCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>(
                "Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        var tree = new Dict();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.BuildDecisionTree",
            (object[] args) =>
            {
                return tree;
            }
        ).Execute();
    }

    [Fact]
    public void BuildTreeCommandPositive()
    {
        var build = new Mock<ITreeBuildable>();
        var path = "../../../file.txt";
        var lists = File.ReadAllLines(path).Select(
            line => line.Split().Select(int.Parse).ToList()
        ).ToList();
        build.Setup(p => p.Tree()).Returns(lists);

        new BuildTreeCommand(build.Object).Execute();

        var tree = IoC.Resolve<Dict>("Game.BuildDecisionTree");

        Assert.NotNull(tree);
        Assert.True(tree.ContainsKey(1));

        var treeNext1 = (Dict)tree[1];
        Assert.True(treeNext1.ContainsKey(2));

        var treeNext2 = (Dict)treeNext1[2];
        Assert.True(treeNext2.ContainsKey(3));

        var treeNext3 = (Dict)treeNext2[3];
        Assert.True(treeNext3.ContainsKey(4));
    }
}
