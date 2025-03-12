using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.Interfaces;

namespace ContentService.DTOs
{
    public class CreateExamRequest
    {
        public required List<string> ExamTypes { get; set; }
        public required string Level { get; set; }
        public required int MaxPoints { get; set; }
        public required List<IExamComponent> ExamComponents { get; set; }
    }
}
