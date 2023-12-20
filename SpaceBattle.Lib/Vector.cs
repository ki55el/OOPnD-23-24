namespace SpaceBattle.Lib;

public class Vector
{
    public int[] nums { get; set; }

    public Vector(int[] nums)
    {
        this.nums = nums;
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
        return new Vector(v1.nums.Select((num, i) => num + v2.nums[i]).ToArray());
    }
}
