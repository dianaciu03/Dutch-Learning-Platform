using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Helpers;
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

        public Exercise(int id, List<string> types, string level, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            this.Id = id;
            this.ExerciseTypes = types.Select(type => EnumConverter.ParseExerciseType(type)).ToList();
            this.Level = EnumConverter.ParseCEFRLevel(level);
            this.Grade = 0.0;
            this._exerciseComponents = exerciseComponents;
        }

        public void DisplayExercise()
        {
            Console.WriteLine($"Exercise {Id} - Type: {string.Join(", ", ExerciseTypes)} - Level: {Level}");
        }
    }
}
