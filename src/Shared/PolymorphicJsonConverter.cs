using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared;

public class PolymorphicJsonConverter<T> : JsonConverter<T> where T : ITypeDiscriminator
{
    private readonly Dictionary<string, Type> _discriminatorTypeMap;

    public PolymorphicJsonConverter()
    {
        var baseType = typeof(T);
        _discriminatorTypeMap =
            typeof(T)
                .Assembly
                .GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract && baseType.IsAssignableFrom(t))
                .ToDictionary(t => t.Name, t => t);
    }
    
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        if (!jsonDocument.RootElement.TryGetProperty("type", out var typeProperty))
        {
            throw new JsonException();
        }

        var discriminator = typeProperty.GetString();
        if (discriminator is null || !_discriminatorTypeMap.TryGetValue(discriminator, out var type))
        {
            throw new JsonException();
        }
        
        var result = (T?)jsonDocument.Deserialize(type, options);

        return result;
    }
    
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, (object)value, options);
}