using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain.ExamComponents
{
    public class GrammarComponent : IExamComponent
    {
        public string id { get; set; }
        public string? ExamId { get; set; }
        public ComponentType ComponentType { get; } = ComponentType.Grammar;

        public ComponentType GetComponentType() => ComponentType;
    }
}
