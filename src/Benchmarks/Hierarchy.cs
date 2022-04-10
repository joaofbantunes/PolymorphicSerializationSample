using Shared;

namespace Benchmarks;

public abstract record HierarchyRoot : ITypeDiscriminator
{
    public abstract string Type { get; }

    public long N1 { get; set; } = 0;
        
    public long N5 { get; set; } = 01234;
    
    public long N10 { get; set; } = 0123456789;
    
    public long N100 { get; set; } = long.MaxValue;
    
    public string U { get; set; } = "u";
    
    public string V { get; set; } = "v";
    
    public string W { get; set; } = "w";
    
    public string X { get; set; } = "x";
    
    public string Y { get; set; } = "y";
    
    public string Z { get; set; } = "z";
    
    public string Uu { get; set; } = "uuuuu";
    
    public string Vv { get; set; } = "vvvvv";
    
    public string Ww { get; set; } = "wwwww";
    
    public string Xx { get; set; } = "xxxxx";
    
    public string Yy { get; set; } = "yyyyy";
    
    public string Zz { get; set; } = "zzzzz";
}

public record A : HierarchyRoot
{
    public override string Type => nameof(A);
}