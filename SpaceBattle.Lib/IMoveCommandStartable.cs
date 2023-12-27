namespace SpaceBattle.Lib;

public interface IMoveCommandStartable
{
    public IUObject Target { get; }
    public Dictionary<string, object> InitialValues { get; }
}
