using ContentService.Interfaces;

namespace ContentService.DTOs
{
    public class UpdateExamRequest
    {
        public int Id { get; set; }
        public List<string> ExamTypes { get; set; }
        public string Level { get; set; }
        public List<IExamComponent> ExamComponents { get; set; }
    }
}
