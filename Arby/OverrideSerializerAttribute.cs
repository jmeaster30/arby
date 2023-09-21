namespace Arby;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OverrideSerializerAttribute : Attribute
{
    readonly Func<object, string> serializer;

    public OverrideSerializerAttribute(Func<object, string> serializer)
    {
        this.serializer = serializer;
    }
}