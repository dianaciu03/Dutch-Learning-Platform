using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain
{
    public class ExamPractice
    {
        public string id { get; set; }
        public int TeacherId { get; set; }
        public string Name { get; set; } 
        public List<ExamType> ExamTypes { get; set; }
        public CEFRLevel Level { get; set; } 
        public int MaxPoints { get; set; } 

        public IEnumerable<IExamComponent> ExamComponents { get; set; }

        [JsonConstructor]
        public ExamPractice(string name, CEFRLevel level, int maxPoints)
        {
            Name = name;
            Level = level;
            MaxPoints = maxPoints;
            ExamComponents = [];
        }
    }
}
