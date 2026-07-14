using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Helpers
{
    public class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var val = reader.GetString();
                if (DateTime.TryParse(val, out var dateTime))
                {
                    return DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
                }
            }
            return DateTime.SpecifyKind(reader.GetDateTime().ToUniversalTime(), DateTimeKind.Utc);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(DateTime.SpecifyKind(value.ToUniversalTime(), DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
    }

    public class NullableUtcDateTimeJsonConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                var val = reader.GetString();
                if (string.IsNullOrEmpty(val)) return null;
                if (DateTime.TryParse(val, out var dateTime))
                {
                    return DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
                }
            }
            return DateTime.SpecifyKind(reader.GetDateTime().ToUniversalTime(), DateTimeKind.Utc);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(DateTime.SpecifyKind(value.Value.ToUniversalTime(), DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }
        }
    }
}
