using Arby;
using Arby.Attributes;

namespace Sample;

public class SecondClass
{
    public double What { get; set; }

    public SecondClass Other { get; set; }

    [Required]
    public string IncredibleString { get; set; }
}