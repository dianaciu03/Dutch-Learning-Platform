using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ContentService.Domain.ExamComponents
{
    public class ReadingComponent : IExamComponent
    {
        public string? id { get; set; }
        public string ExamId { get; set; }
        public string GivenText { get; private set; }
        public List<QuestionAnswerTuple> Questions { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ComponentType ComponentType { get; } = ComponentType.Reading;

        public ReadingComponent() { }

        public ReadingComponent(string examId, string givenText, List<QuestionAnswerTuple> questions)
        {
            ExamId = examId;
            GivenText = givenText;
            Questions = questions;
        }
        public ComponentType GetComponentType() => ComponentType;
    }
}
