using Shared;

namespace Benchmarks;

public abstract record HierarchyRoot(int SomeSharedProperty) : ITypeDiscriminator
{
    public abstract string Type { get; }
}

public record A(int SomeSharedProperty) : HierarchyRoot(SomeSharedProperty)
{
    public override string Type => nameof(A);
}