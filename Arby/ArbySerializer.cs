using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Arby.Attributes;

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

    private static string GenIndentation(int level, int offset, ArbySerializerOptions? options)
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

        for (var i = 0; i < offset; i++)
        {
            result += " ";
        }

        return result;
    }

    private static string SerializeInternal(object o, int level, int offset, ArbySerializerOptions? options = null)
    {
        switch (o)
        {
            case null:
                return "null";
            case ITuple tuple:
            {
                var result = "";

                if (tuple.Length == 0)
                    return result;
                
                for (var i = 0; i < tuple.Length - 1; i++)
                {
                    result += GenIndentation(level, offset, options);
                    result += "- " + SerializeInternal(tuple[i], level, 2, options) + "\n";
                }

                result += GenIndentation(level, offset, options);
                result += "- " + SerializeInternal(tuple[tuple.Length - 1], level, 2, options);

                return result;
            }
            case ValueType or string:
                return o switch
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
            case IEnumerable enumerable:
            {
                var result = "";
                var values = enumerable.GetEnumerator();
                if (!values.MoveNext()) break;

                var current = values.Current;

                while (values.MoveNext())
                {
                    result += GenIndentation(level, offset, options);
                    result += "- " + SerializeInternal(current, level, 2, options) + "\n";
                    current = values.Current;
                }

                result += GenIndentation(level, offset, options);
                result += "- " + SerializeInternal(current, level, 2, options);

                return result;
            }
            default:
            {
                var result = "";
                var first = true;
                var members = o.GetType().GetMembers();
                for (var i = 0; i < members.Length; i++)
                {
                    var member = members[i];
                    
                    if (member.CustomAttributes.Any(x => x.AttributeType == typeof(ExcludeAttribute)) ||
                        member.MemberType is not MemberTypes.Property) continue;

                    result += first ? "" : GenIndentation(level, offset, options);
                    result += $"{member.Name}: ";
                    var propertyValue = ((PropertyInfo)member).GetValue(o);
                    result += propertyValue switch
                    {
                        ValueType or string or null => Serialize(propertyValue, options),
                        _ => $"\n{(first ? "" : GenIndentation(level, offset, options))}{SerializeInternal(propertyValue, level + 1, offset, options)}"
                    };
                    result += i == members.Length - 1 ? "" : "\n";

                    if (first) first = !first;
                }

                return result;
            }
        }

        throw new NotImplementedException($"WE GOT HERE!!! <{o.GetType()}> {o}");
    }
}
