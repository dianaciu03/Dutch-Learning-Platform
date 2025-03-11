using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain
{
    public class Exercise
    {
        public int Id { get; private set; }
        public int TeacherId { get; private set; }
        public List<ExerciseType> ExerciseTypes { get; private set; }
        public CEFRLevel Level { get; private set; } 
        public int MaxPoints { get; private set; } 

        private readonly IEnumerable<IExerciseComponent> _exerciseComponents;

        public Exercise(List<ExerciseType> types, CEFRLevel level, int maxPoints, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            ExerciseTypes = types;
            Level = level;
            MaxPoints = maxPoints;
            _exerciseComponents = exerciseComponents;
        }

        public void DisplayExercise()
        {
            Console.WriteLine($"Exercise {Id} - Type: {string.Join(", ", ExerciseTypes)} - Level: {Level}");
        }
    }
}
