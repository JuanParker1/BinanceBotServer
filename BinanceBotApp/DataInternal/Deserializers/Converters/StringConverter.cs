using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace BinanceBotApp.DataInternal.Deserializers.Converters
{
    public class StringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, 
            JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return reader.TryGetUInt64(out var value) 
                        ? value.ToString() 
                        : "null";
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.StartArray:
                    var values = new List<string>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        values.Add(reader.GetString());
                    return string.Join(',', values);
                case JsonTokenType.Null:
                    return "null";
                default:
                    throw new JsonException();
                
            }
        }
 
        public override void Write(Utf8JsonWriter writer, string value, 
            JsonSerializerOptions options) =>
            writer.WriteStringValue(value);
    }
}