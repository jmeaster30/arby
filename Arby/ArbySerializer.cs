using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Arby;

public static class ArbySerializer
{
    public static string Serialize(string o, ArbySerializerOptions? options = null) => $"'{o}'";
    public static string Serialize(sbyte o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(byte o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(short o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(ushort o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(int o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(uint o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(long o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(ulong o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(char o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(float o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(double o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(decimal o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(bool o, ArbySerializerOptions? options = null) => $"{o}";
    public static string Serialize(Enum e, ArbySerializerOptions? options = null) => $"{e}";
    
    public static string Serialize(object o, ArbySerializerOptions? options = null)
    {
        return SerializeInternal(o, 0, 0, options);
    }

    private static string GenIndentation(int level, ArbySerializerOptions? options)
    {
        var result = "";
        for(var i = 0; i < level; i++)
        {
            for (var j = 0; j < (options?.IndentSize ?? 1); j++)
            {
                result += options?.IndentCharacter switch
                {
                    IndentCharacter.Space => " ",
                    IndentCharacter.Tab or null => "\t",
                    _ => throw new NotImplementedException(),
                };
            }
        }

        return result;
    }

    private static string SerializeInternal(object o, int level, int offset, ArbySerializerOptions? options = null)
    {
        var result = GenIndentation(level, options);

        switch (o)
        {
            case null:
                result += "null";
                break;
            case ValueType or string:
                result += o switch
                {
                    string s => Serialize(s, options),
                    sbyte s => Serialize(s, options),
                    byte s => Serialize(s, options),
                    short s => Serialize(s, options),
                    ushort s => Serialize(s, options),
                    int s => Serialize(s, options),
                    uint s => Serialize(s, options),
                    long s => Serialize(s, options),
                    ulong s => Serialize(s, options),
                    char s => Serialize(s, options),
                    float s => Serialize(s, options),
                    double s => Serialize(s, options),
                    decimal s => Serialize(s, options),
                    bool s => Serialize(s, options),
                    Enum e => Serialize(e, options),
                    _ => throw new NotImplementedException($"WE GOT HERE {o.GetType()}")
                };
                break;
            case IEnumerable enumerable:
            {
                // TODO resetting here feels odd but it shouldn't cause issues.
                result = "";
                foreach(var value in enumerable)
                {
                    result += GenIndentation(level, options);
                    result += "- " + SerializeInternal(value, 0, 2, options) + "\n";
                }

                break;
            }
            default:
            {
                result = "";
                foreach (var member in o.GetType().GetMembers())
                {
                    if (member.CustomAttributes.Any(x => x.AttributeType == typeof(ExcludeAttribute)) ||
                        member.MemberType is not MemberTypes.Property) continue;
                    
                    result += GenIndentation(level, options);

                    for (var i = 0; i < offset; i++)
                    {
                        result += " ";
                    }
                        
                    result += $"{member.Name}: ";
                    var propertyValue = ((PropertyInfo)member).GetValue(o);
                    result += propertyValue switch
                    {
                        ValueType or string => Serialize(propertyValue, options),
                        _ => $"\n{SerializeInternal(propertyValue, level + 1, offset, options)}"
                    };
                    result += "\n";
                }
                break;
            }
        }

        return result;
    }
}
