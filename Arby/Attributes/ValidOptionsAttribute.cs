namespace Arby.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class ValidOptionsAttribute : Attribute
{
    private readonly object?[] options;

    public ValidOptionsAttribute(params object?[] options)
    {
        this.options = options;
    }
}