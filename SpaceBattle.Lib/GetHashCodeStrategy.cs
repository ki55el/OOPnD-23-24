namespace SpaceBattle.Lib;

public class GetHashCodeStrategy : IStrategy
{
    public object Build(params object[] args)
    {
        var Types = ((IEnumerable<Type>)args[0]).OrderBy(x => x.GetHashCode());
        unchecked
        {
            return Types.Aggregate(31, (total, next) => HashCode.Combine(total, next));
        }
    }
}
