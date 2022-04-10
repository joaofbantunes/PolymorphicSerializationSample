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
    
    private static readonly JsonSerializerOptions BasicPolymorphicJsonSerializerOptions = new()
    {
        Converters = {new BasicPolymorphicJsonConverter<HierarchyRoot>()},
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    private static readonly JsonSerializerOptions TweakedPolymorphicJsonSerializerOptions = new()
    {
        Converters = {new TweakedPolymorphicJsonConverter<HierarchyRoot>()},
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly IReadOnlyCollection<HierarchyRoot> Sample
        = Enumerable.Range(0, 10000).Select(i => new A()).ToArray();

    private static readonly string SerializedSample
        = JsonSerializer.Serialize(Sample, TweakedPolymorphicJsonSerializerOptions);

    [Benchmark(Baseline = true), BenchmarkCategory("Serialization")]
    public string NonPolymorphicSerialization()
        => JsonSerializer.Serialize(Sample, NonPolymorphicJsonSerializerOptions);

    [Benchmark, BenchmarkCategory("Serialization")]
    public string BasicPolymorphicSerialization()
        => JsonSerializer.Serialize(Sample, BasicPolymorphicJsonSerializerOptions);
    
    [Benchmark, BenchmarkCategory("Serialization")]
    public string TweakedPolymorphicSerialization()
        => JsonSerializer.Serialize(Sample, TweakedPolymorphicJsonSerializerOptions);
    
    [Benchmark(Baseline = true), BenchmarkCategory("Deserialization")]
    public IReadOnlyCollection<HierarchyRoot> NonPolymorphicDeserialization()
        => JsonSerializer.Deserialize<IReadOnlyCollection<A>>(SerializedSample, NonPolymorphicJsonSerializerOptions)!;

    [Benchmark, BenchmarkCategory("Deserialization")]
    public IReadOnlyCollection<HierarchyRoot> BasicPolymorphicDeserialization()
        => JsonSerializer.Deserialize<IReadOnlyCollection<HierarchyRoot>>(SerializedSample, BasicPolymorphicJsonSerializerOptions)!;
    
    [Benchmark, BenchmarkCategory("Deserialization")]
    public IReadOnlyCollection<HierarchyRoot> TweakedPolymorphicDeserialization()
        => JsonSerializer.Deserialize<IReadOnlyCollection<HierarchyRoot>>(SerializedSample, TweakedPolymorphicJsonSerializerOptions)!;

    internal static void DoSanityChecks()
    {
        var benchmark = new Benchmark();
        
        if (SerializedSample != benchmark.NonPolymorphicSerialization())
            throw new Exception($"Something's wrong with benchmark {nameof(NonPolymorphicSerialization)}");
        
        if (SerializedSample != benchmark.TweakedPolymorphicSerialization())
            throw new Exception($"Something's wrong with benchmark {nameof(TweakedPolymorphicSerialization)}");
        
        if (Sample.Except(benchmark.NonPolymorphicDeserialization()).Count() != 0)
            throw new Exception($"Something's wrong with benchmark {nameof(NonPolymorphicDeserialization)}");
        
        if (Sample.Except(benchmark.TweakedPolymorphicDeserialization()).Count() != 0)
            throw new Exception($"Something's wrong with benchmark {nameof(TweakedPolymorphicDeserialization)}");
    }
}
