using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;

namespace ContentService.DTOs
{
    public class CreateReadingComponentRequest
    {
        public string? id { get; set; }
        public required string ExamId { get; set; }
        public required string Text { get; set; }

        public required List<QuestionAnswerTuple> Questions { get; set; }
    }
}
