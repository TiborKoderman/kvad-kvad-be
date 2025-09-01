[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class PgCompositeAttribute : Attribute
{
    public string TypeName { get; }
    public PgCompositeAttribute(string typeName) => TypeName = typeName; // e.g. "public.dim7"
}