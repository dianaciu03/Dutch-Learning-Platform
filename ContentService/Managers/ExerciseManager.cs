using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.Interfaces;

namespace ContentService.Managers
{
    public class ExerciseManager : IExerciseManager
    {
        private readonly List<Exercise> _exercises = [];

        // Create
        public void AddExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            var exercise = new Exercise(id, types, level, exerciseComponents);
            _exercises.Add(exercise);
        }

        // Read
        public Exercise GetExerciseById(int id)
        {
            return _exercises.FirstOrDefault(e => e.Id == id);
        }

        public List<Exercise> GetAllExercises()
        {
            return _exercises;
        }

        // Update
        public bool UpdateExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            var exercise = GetExerciseById(id);
            if (exercise == null)
            {
                return false;
            }

            // Assuming Exercise has methods to update its properties
            exercise = new Exercise(id, types, level, exerciseComponents);
            return true;
        }

        // Delete
        public bool DeleteExercise(int id)
        {
            var exercise = GetExerciseById(id);
            if (exercise == null)
            {
                return false;
            }

            _exercises.Remove(exercise);
            return true;
        }
    }
}
