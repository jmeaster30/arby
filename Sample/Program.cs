// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Arby;
using Sample;

Console.WriteLine(">>>> Serialize");
Console.WriteLine(ArbySerializer.Serialize(1234));
Console.WriteLine(ArbySerializer.Serialize(true));
Console.WriteLine(ArbySerializer.Serialize("this is a test"));
Console.WriteLine(ArbySerializer.Serialize((object)"this is another test"));
Console.WriteLine(ArbySerializer.Serialize(new List<string> { "this", "is", "a", "good test"}));
Console.WriteLine(ArbySerializer.Serialize(new [] { "this", "is", "a", "good test"}));
Console.WriteLine(ArbySerializer.Serialize(MyEnum.ALL)); // TODO get rid of boxing conversion
Console.WriteLine(ArbySerializer.Serialize(("what", 123, "the"))); // TODO get rid of boxing conversion
Console.WriteLine(ArbySerializer.Serialize(new SecondClass { What = 1.2, IncredibleString = "holy cow"}));
Console.WriteLine(ArbySerializer.Serialize(new MyClass
{
    Field1 = "OH WOW",
    ImportantEnum = MyEnum.WAAA,
    Ignored = true,
    Option = "alpha",
    SecondO = new List<SecondClass>
    {
        new() { What = 13.12, IncredibleString = "good", Other = new SecondClass 
            {
                IncredibleString = "Wow!!!!!",
                What = 5432,
            } 
        },
        new() { What = 1234, IncredibleString = "oh my god this is working :)"}
    }
}, new ArbySerializerOptions { IndentCharacter = IndentCharacter.Space, IndentSize = 2 }));

Console.WriteLine("\n>>>> Deserialize");
Console.WriteLine(JsonSerializer.Serialize(ArbyDeserializer.Deserialize<int>("1234")));
Console.WriteLine(JsonSerializer.Serialize(ArbyDeserializer.Deserialize<MyClass>(ArbySerializer.Serialize(new MyClass
{
    Field1 = "OH WOW",
    ImportantEnum = MyEnum.WAAA,
    Ignored = true,
    Option = "alpha",
}, new ArbySerializerOptions { IndentCharacter = IndentCharacter.Space, IndentSize = 2 }))));
