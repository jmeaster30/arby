namespace Arby;

public enum IndentCharacter
{
    Tab,
    Space
}

public class ArbySerializerOptions
{
    public int IndentSize { get; set; } = 1;
    public IndentCharacter IndentCharacter { get; set; } = IndentCharacter.Tab;
}