using System.Text.Json;
using App;
using Shared;

var jsonSerializerOptions = new JsonSerializerOptions
{
    Converters = {new PolymorphicJsonConverter<HierarchyRoot>()},
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

Console.WriteLine("Serialized:");
Console.WriteLine(JsonSerializer.Serialize(
    new HierarchyRoot[] {new A(1, 2), new B(3, 4)},
    jsonSerializerOptions));

Console.WriteLine();

using var jsonSample = File.OpenRead("sample.json");
var deserialized = JsonSerializer.Deserialize<IReadOnlyCollection<HierarchyRoot>>(jsonSample, jsonSerializerOptions)!;
Console.WriteLine("Deserialized:");
Console.WriteLine(string.Join(Environment.NewLine, deserialized.Select(d => d.ToString())));