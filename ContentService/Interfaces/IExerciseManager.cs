using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.DTOs;

namespace ContentService.Interfaces
{
    public interface IExerciseManager
    {
        bool CreateExercise(CreateExerciseRequest request);
        ComponentResponse CreateExerciseComponent(CreateExerciseComponentRequest request);
        Exercise GetExerciseById(int id);
        List<Exercise> GetAllExercises();
        bool UpdateExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents);
        bool DeleteExercise(int id);
    }
}
