public abstract class BaseVariable
{
    public required Guid Id { get; set; }

    public abstract string GetValue();
}

public class TagVariable : BaseVariable
{
    public required Tag Tag { get; set; }

    public override string GetValue()
    {
        return Tag.Curr.Value.ToString();
    }
}
