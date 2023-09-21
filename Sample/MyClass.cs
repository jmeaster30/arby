using Arby;

namespace Sample;

public class MyClass
{
    [Required]
    public string? Field1 { get; set; }

    public MyEnum ImportantEnum { get; set; }

    private int SomeSecretNumber { get; set; } = 420;
    
    [Exclude]
    public bool Ignored { get; set; }
    
    [ValidOptions("dev", "alpha", "beta", "prod")]
    public string Option { get; set; }
    
    public List<SecondClass> SecondO { get; set; } 
}