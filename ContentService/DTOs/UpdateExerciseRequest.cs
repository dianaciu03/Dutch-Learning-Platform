using ContentService.Interfaces;

namespace ContentService.DTOs
{
    public class UpdateExerciseRequest
    {
        public List<string> ExerciseTypes { get; set; }
        public string Level { get; set; }
        public List<IExerciseComponent> ExerciseComponents { get; set; }
    }
}
