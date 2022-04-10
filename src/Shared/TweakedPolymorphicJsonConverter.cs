using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared;

public class TweakedPolymorphicJsonConverter<T> : JsonConverter<T> where T : ITypeDiscriminator
{
    private static readonly Type RootType = typeof(T);
    
    private readonly Dictionary<string, Type> _discriminatorTypeMap;

    public TweakedPolymorphicJsonConverter()
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
        // To find the actual type, we need to find the "type" property.
        // As going through the reader changes it, we want to clone it,
        // so it remains in its initial state and we can use it to deserialize when we know the actual type.
        // As Utf8JsonReader is a struct, assigning to another variable effectively clones it.
        var readerClone = reader;

        var type = RootType;
        
        if (readerClone.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        while(readerClone.Read())
        {
            if (readerClone.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = readerClone.GetString();
            if (propertyName == "type")
            {
                readerClone.Read();
                var discriminator = readerClone.GetString();
                if (discriminator is null || !_discriminatorTypeMap.TryGetValue(discriminator, out type))
                {
                    throw new JsonException();
                }

                break;
            }

            readerClone.Read();
            readerClone.Skip();
        }

        if (type == RootType)
        {
            throw new JsonException();
        }
        
        return (T?)JsonSerializer.Deserialize(ref reader, type, options);
    }
    
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, (object)value, options);
}