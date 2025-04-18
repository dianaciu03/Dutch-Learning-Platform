using ContentService.Domain;

namespace ContentService.Interfaces
{
    public interface IExamPracticeRepository
    {
        Task<string?> SaveExamPracticeAsync(ExamPractice exam);
    }
}
