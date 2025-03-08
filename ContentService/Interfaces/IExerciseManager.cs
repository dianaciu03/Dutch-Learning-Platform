using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;

namespace ContentService.Interfaces
{
    public interface IExerciseManager
    {
       void AddExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents);
        Exercise GetExerciseById(int id);
        List<Exercise> GetAllExercises();
        bool UpdateExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents);
        bool DeleteExercise(int id);
    }
}
