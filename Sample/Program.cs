// See https://aka.ms/new-console-template for more information

using arby;
using Sample;

Console.WriteLine(Arby.Serialize(1234));
Console.WriteLine(Arby.Serialize(true));
Console.WriteLine(Arby.Serialize("this is a test"));
Console.WriteLine(Arby.Serialize((object)"this is another test"));
Console.WriteLine(Arby.Serialize(new List<string> { "this", "is", "a", "good test"}));
Console.WriteLine(Arby.Serialize(new SecondClass { What = 1.2, IncredibleString = "holy cow"}));