using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain
{
    public class ExamPractice
    {
        public int Id { get; private set; }
        public int TeacherId { get; private set; }
        public List<ExamType> ExamTypes { get; set; }
        public CEFRLevel Level { get; set; } 
        public int MaxPoints { get; set; } 

        public IEnumerable<IExamComponent> ExamComponents { get; set; }

        public ExamPractice(List<ExamType> types, CEFRLevel level, int maxPoints, IEnumerable<IExamComponent> examComponents)
        {
            ExamTypes = types;
            Level = level;
            MaxPoints = maxPoints;
            ExamComponents = examComponents;
        }

        public void DisplayExamPractice()
        {
            Console.WriteLine($"Exercise {Id} - Type: {string.Join(", ", ExamTypes)} - Level: {Level}");
        }
    }
}
