using System.Collections;
using System.Runtime.CompilerServices;

namespace arby;

public static class Arby
{
    #region serializer
    public static string Serialize(string o) => $"'{o}'";
    public static string Serialize(sbyte o) => $"{o}";
    public static string Serialize(byte o) => $"{o}";
    public static string Serialize(short o) => $"{o}";
    public static string Serialize(ushort o) => $"{o}";
    public static string Serialize(int o) => $"{o}";
    public static string Serialize(uint o) => $"{o}";
    public static string Serialize(long o) => $"{o}";
    public static string Serialize(ulong o) => $"{o}";
    public static string Serialize(char o) => $"{o}";
    public static string Serialize(float o) => $"{o}";
    public static string Serialize(double o) => $"{o}";
    public static string Serialize(decimal o) => $"{o}";
    public static string Serialize(bool o) => $"{o}";
    
    public static string Serialize(object o)
    {
        return SerializeInternal(o, 0);
    }

    private static string SerializeInternal(object o, int level)
    {
        var result = "";
        
        // add indentations for level
        for(var i = 0; i < level; i++)
        {
            result += "\t";
        }

        switch (o)
        {
            case ValueType or string:
                result += o switch
                {
                    string s => Serialize(s),
                    sbyte s => Serialize(s),
                    byte s => Serialize(s),
                    short s => Serialize(s),
                    ushort s => Serialize(s),
                    int s => Serialize(s),
                    uint s => Serialize(s),
                    long s => Serialize(s),
                    ulong s => Serialize(s),
                    char s => Serialize(s),
                    float s => Serialize(s),
                    double s => Serialize(s),
                    decimal s => Serialize(s),
                    bool s => Serialize(s),
                    _ => "" // do nothing
                };
                break;
            case IEnumerable enumerable:
            {
                // TODO resetting here feels odd but it shouldn't cause issues.
                result = "";
                foreach(var value in enumerable)
                {
                    for(var i = 0; i < level; i++)
                    {
                        result += "\t";
                    }
                    result += "- " + Serialize(value) + "\n";
                }

                break;
            }
            default:
                throw new NotImplementedException(
                    $"Serialization for type '{o.GetType()}' has not been implemented yet :(");
        }

        return result;
    }
    #endregion

    #region deserialize
    public static T Deserialize<T>(string s)
    {
        return Activator.CreateInstance<T>();
    }
    
    public static Dictionary<String, object?> Deserialize(string s)
    {
        return new();
    }
    #endregion
}
