namespace Arby;

public static class ArbyDeserializer
{
    public static T Deserialize<T>(string s)
    {
        return Activator.CreateInstance<T>();
    }
    
    public static Dictionary<String, object?> Deserialize(string s)
    {
        return new();
    }
}