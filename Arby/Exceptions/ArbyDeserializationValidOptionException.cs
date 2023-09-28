namespace Arby.Exceptions;

public class InvalidOptionArbyDeserializationException : Exception
{
    public object DeserializedOption { get; set; }
    public object?[] ExpectedOptions { get; set; }
}