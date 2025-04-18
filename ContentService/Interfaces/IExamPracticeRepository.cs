using ContentService.Domain;

namespace ContentService.Interfaces
{
    public interface IExamPracticeRepository
    {
        Task SaveExamPracticeAsync(ExamPractice exam);
    }
}
