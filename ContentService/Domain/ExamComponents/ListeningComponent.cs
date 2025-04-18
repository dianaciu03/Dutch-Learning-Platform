using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain.ExamComponents
{
    public class ListeningComponent : IExamComponent
    {
        public string id { get; set; }
        public string? ExamId { get; set; }
        public ComponentType ComponentType { get; } = ComponentType.Listening;

        public ComponentType GetComponentType() => ComponentType;
    }
}
