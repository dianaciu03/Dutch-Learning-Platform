using ContentService.Domain.ExamComponents;
using ContentService.Domain;
using ContentService.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ContentService.Helpers
{
    public class ExamComponentConverter : JsonConverter<IExamComponent>
    {
        public override IExamComponent ReadJson(JsonReader reader, Type objectType, IExamComponent? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            // Avoid re-serializing objects that are already deserialized
            if (existingValue != null)
            {
                return existingValue;
            }

            var componentType = jo["ComponentType"]?.ToString();

            // Switch case based on component type
            return componentType switch
            {
                "Reading" => jo.ToObject<ReadingComponent>(serializer),
                "Grammar" => jo.ToObject<GrammarComponent>(serializer),
                // Add other types as needed
                _ => throw new NotSupportedException($"Unknown component type: {componentType}")
            };
        }

        public override void WriteJson(JsonWriter writer, IExamComponent value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override bool CanWrite => true;


    }
}
