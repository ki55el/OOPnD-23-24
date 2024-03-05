namespace SpaceBattle.Lib;

public interface IStrategy
{
    public object Build(params object[] args);
}
