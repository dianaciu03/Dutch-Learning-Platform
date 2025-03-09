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
        public List<ExerciseType> ExerciseTypes { get; private set; }
        public CEFRLevel Level { get; private set; } 
        public double Grade { get; private set; } 

        private readonly IEnumerable<IExerciseComponent> _exerciseComponents;

        public Exercise(List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            this.ExerciseTypes = types;
            this.Level = level;
            this.Grade = 0.0;
            this._exerciseComponents = exerciseComponents;
        }

        public void DisplayExercise()
        {
            Console.WriteLine($"Exercise {Id} - Type: {string.Join(", ", ExerciseTypes)} - Level: {Level}");
        }
    }
}
