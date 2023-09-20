using arby;

namespace Sample;

public class MyClass
{
    [Required]
    public string? Field1 { get; set; }

    public MyEnum ImportantEnum { get; set; }
    
    private int SomeSecretNumber { get; set; }
    
    [Exclude]
    public bool Ignored { get; set; }
    
    [ValidOptions("dev", "alpha", "beta", "prod")]
    public string Option { get; set; }
}