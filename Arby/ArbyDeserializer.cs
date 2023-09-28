using System.Text.Json;
using System.Text.RegularExpressions;
using Arby.Exceptions;

namespace Arby;

public static class ArbyDeserializer
{
    private static Regex RegexString = new(@"^'[^']*'$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    private static Regex RegexNumber = new(@"^[+-]?((\d+(\.\d*)?)|(\.\d+))$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    private static Regex RegexEnum = new(@"^[a-zA-Z_]\w*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    
    public static T Deserialize<T>(string s)
    {
        return (T)(Deserialize(typeof(T),s.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(), 0, -1).Item1);
    }

    private static (object, int) Deserialize(Type type, string[] rows, int rowIndex, int indentationLevel)
    {
        var result = Activator.CreateInstance(type);
        var index = rowIndex;
        var expectedIndentationLevel = indentationLevel;
        string? lastLabel = null;
        var isObject = false; 
        for (; index < rows.Length; index++)
        {
            var (foundIndentLevel, isArray, label, value) = parseRow(rows[index]);
            if (expectedIndentationLevel == -1)
            {
                expectedIndentationLevel = foundIndentLevel;
            }
            else if (expectedIndentationLevel < foundIndentLevel && lastLabel != null)
            {
                var property = type.GetProperty(lastLabel);
                var nextType = property.PropertyType;
                var (subValue, newRowIndex) = Deserialize(nextType, rows, index, foundIndentLevel);
                index = newRowIndex;
                property.SetValue(result, subValue);
            }
            else if (foundIndentLevel > expectedIndentationLevel && lastLabel == null)
            {
                break;
            }
            else if (foundIndentLevel != expectedIndentationLevel)
            {
                throw new ArbyDeserializationException("Bad Indentation :(");
            }

            if (label == null && value != null && !isObject)
            {
                // we are parsing a value
                result = DeserializeValue(type, value);
                index += 1;
                if (index != rows.Length)
                {
                    throw new ArbyDeserializationException("Parsed necessary value but there was more :(");
                }
                break;
            }
            else if (label != null && value == null)
            {
                isObject = true;
                // recurse
                lastLabel = label;
            }
            else if (label != null && value != null)
            {
                isObject = true;
                var property = type.GetProperty(label);
                var memberValue = DeserializeValue(property.PropertyType, value);
                property.SetValue(result, memberValue);
                lastLabel = null;
            }
        }
        
        return (result, index);
    }

    private static object DeserializeValue(Type type, string value)
    {
        if (RegexString.IsMatch(value) && typeof(string) == type)
        {
            return value[1..^1];
        }
        if (RegexString.IsMatch(value) && typeof(char) == type && value.Length is 2 or 3)
        {
            return value.Length is 2 ? "" : value[1..^1][0];
        }
        switch (value.ToLower())
        {
            case "true" or "false" when type == typeof(bool):
                return value.ToLower() == "true";
            case "null":
                return null;
        }
        if (RegexEnum.IsMatch(value) && type.IsAssignableTo(typeof(Enum)))
        {
            var enumOptions = type.GetFields().Select(field => field.Name).Where(enumValue => !enumValue.Equals("value__"));
            if (enumOptions.Contains(value))
            {
                return Enum.Parse(type, value);
            }
            
            throw new ArbyDeserializationException($"Error parsing enum. Got '{value}' expected one of [{string.Join(',', enumOptions)}]");
        }

        if (!RegexNumber.IsMatch(value)) throw new ArbyDeserializationException($"Expected value of type {type} but got '{value}'");

        if (type == typeof(sbyte))
        {
            return sbyte.Parse(value);
        }

        if (type == typeof(byte))
        {
            return byte.Parse(value);
        }

        if (type == typeof(short))
        {
            return short.Parse(value);
        }

        if (type == typeof(ushort))
        {
            return ushort.Parse(value);
        }

        if (type == typeof(int))
        {
            return int.Parse(value);
        }

        if (type == typeof(uint))
        {
            return uint.Parse(value);
        }

        if (type == typeof(long))
        {
            return long.Parse(value);
        }

        if (type == typeof(ulong))
        {
            return ulong.Parse(value);
        }
            
        if (type == typeof(float))
        {
            return float.Parse(value);
        }

        if (type == typeof(double))
        {
            return double.Parse(value);
        }

        if (type == typeof(decimal))
        {
            return decimal.Parse(value);
        }

        throw new ArbyDeserializationException($"Expected type {type} but got value '{value}'");
    }

    private static (int, bool, string?, string?) parseRow(string row)
    {
        var whitespace = row[..^row.TrimStart().Length];
        var indentLevel = whitespace.Sum(c => c switch
        {
            ' ' => 1,
            '\t' => 4,
            _ => 0,
        });

        var isArray = row[whitespace.Length] == '-';
        indentLevel += 2;

        var data = row[(isArray ? whitespace.Length + 2 : whitespace.Length)..];
        var splitLabel = data.Split(":", 2);

        string? label = null;
        string? value;

        if (data.Contains(':'))
        {
            label = splitLabel[0];
            value = splitLabel.Length == 2 ? splitLabel[1].Trim() : null;
        }
        else
        {
            value = splitLabel[0];
        }
        
        return (indentLevel, isArray, label, value);
    }
}