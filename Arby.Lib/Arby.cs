namespace arby;

public static class Arby
{
    public static string Serialize(Object o)
    {
        return "";
    }

    public static T Deserialize<T>(string s)
    {
        return Activator.CreateInstance<T>();
    }
}
