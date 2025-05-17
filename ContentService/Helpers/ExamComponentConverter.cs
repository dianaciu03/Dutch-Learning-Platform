using System.Text.Json;
using System.Text.Json.Serialization;
using ContentService.Domain.ExamComponents;
using ContentService.Domain;
using ContentService.Interfaces;

namespace ContentService.Helpers
{
    public class ExamComponentConverter : JsonConverter<IExamComponent>
    {
        public override IExamComponent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;
            var componentType = (ComponentType) root.GetProperty(nameof(IExamComponent.ComponentType)).GetInt32();

            var json = root.GetRawText();

            return componentType switch
            {
                ComponentType.Reading => JsonSerializer.Deserialize<ReadingComponent>(json),
                ComponentType.Grammar => JsonSerializer.Deserialize<GrammarComponent>(json),
                // Add other types as needed
                _ => throw new NotSupportedException($"Unknown component type: {componentType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, IExamComponent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType());
        }
    }
}
