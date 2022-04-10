using Shared;

namespace App;

public abstract record HierarchyRoot(int SomeSharedProperty) : ITypeDiscriminator
{
    public abstract string Type { get; }
}

public record A(int SomeSharedProperty, int SomeASpecificProperty) : HierarchyRoot(SomeSharedProperty)
{
    public override string Type => nameof(A);
}

public record B(int SomeSharedProperty, int SomeBSpecificProperty) : HierarchyRoot(SomeSharedProperty)
{
    public override string Type => nameof(B);
}