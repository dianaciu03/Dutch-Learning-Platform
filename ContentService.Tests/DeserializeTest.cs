using System.Text.Json;
using ContentService.Domain;
using ContentService.Domain.ExamComponents;

namespace ContentService.Tests
{
    public class DeserializeTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldPreserveJsonStructureAfterSerialization()
        {
            var json = """
                {
                    "id": null,
                    "ExamId": "E420",
                    "GivenText": "Text",
                    "Questions": [
                        {
                            "Question": "Question",
                            "Answer": "Answer"
                        }
                    ],
                    "ComponentType": 2
                }
                """;

            var deserializedObject = JsonSerializer.Deserialize<ReadingComponent>(json);
            Assert.NotNull(deserializedObject);
            Assert.Null(deserializedObject.id);
            Assert.Equal("E420", deserializedObject.ExamId);
            Assert.Equal("Text", deserializedObject.GivenText);
            Assert.Equal(ComponentType.Reading, deserializedObject.ComponentType);

            Assert.NotNull(deserializedObject.Questions);
            Assert.Single(deserializedObject.Questions);
            Assert.Equal("Question", deserializedObject.Questions[0].Question);
            Assert.Equal("Answer", deserializedObject.Questions[0].Answer);
        }
    }
}