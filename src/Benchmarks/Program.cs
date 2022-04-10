using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Benchmarks;
using Shared;

Benchmark.DoSanityChecks();
_ = BenchmarkRunner.Run<Benchmark>();

[RankColumn]
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Benchmark
{
    private static readonly JsonSerializerOptions NonPolymorphicJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    private static readonly JsonSerializerOptions PolymorphicJsonSerializerOptions = new()
    {
        Converters = {new PolymorphicJsonConverter<HierarchyRoot>()},
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly IReadOnlyCollection<HierarchyRoot> Sample
        = Enumerable.Range(0, 10000).Select(i => new A(i)).ToArray();

    private static readonly string SerializedSample
        = JsonSerializer.Serialize(Sample, PolymorphicJsonSerializerOptions);

    [Benchmark(Baseline = true), BenchmarkCategory("Serialization")]
    public string NonPolymorphicSerialization()
        => JsonSerializer.Serialize(Sample, NonPolymorphicJsonSerializerOptions);

    [Benchmark, BenchmarkCategory("Serialization")]
    public string PolymorphicSerialization()
        => JsonSerializer.Serialize(Sample, PolymorphicJsonSerializerOptions);
    
    [Benchmark(Baseline = true), BenchmarkCategory("Deserialization")]
    public IReadOnlyCollection<HierarchyRoot> NonPolymorphicDeserialization()
        => JsonSerializer.Deserialize<IReadOnlyCollection<A>>(SerializedSample, NonPolymorphicJsonSerializerOptions)!;

    [Benchmark, BenchmarkCategory("Deserialization")]
    public IReadOnlyCollection<HierarchyRoot> PolymorphicDeserialization()
        => JsonSerializer.Deserialize<IReadOnlyCollection<HierarchyRoot>>(SerializedSample, PolymorphicJsonSerializerOptions)!;

    internal static void DoSanityChecks()
    {
        var benchmark = new Benchmark();
        
        if (SerializedSample != benchmark.NonPolymorphicSerialization())
            throw new Exception($"Something's wrong with benchmark {nameof(NonPolymorphicSerialization)}");
        
        if (SerializedSample != benchmark.PolymorphicSerialization())
            throw new Exception($"Something's wrong with benchmark {nameof(PolymorphicSerialization)}");
        
        if (Sample.Except(benchmark.NonPolymorphicDeserialization()).Count() != 0)
            throw new Exception($"Something's wrong with benchmark {nameof(NonPolymorphicDeserialization)}");
        
        if (Sample.Except(benchmark.PolymorphicDeserialization()).Count() != 0)
            throw new Exception($"Something's wrong with benchmark {nameof(PolymorphicDeserialization)}");
    }
}
