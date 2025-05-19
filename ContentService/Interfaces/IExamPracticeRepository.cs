using ContentService.Domain;

namespace ContentService.Interfaces
{
    public interface IExamPracticeRepository
    {
        Task<string?> SaveExamPracticeAsync(ExamPractice exam);
        Task<string?> SaveExamComponentAsync(IExamComponent component);
        Task<ExamPractice?> GetExamPracticeByIdAsync(string examId);
        Task<List<ExamPractice>> GetAllExamPracticesAsync();
        Task<int> DeleteAllExamPracticesAsync();
        Task<bool> DeleteExamPracticeByIdAsync(string examId);
    }
}
