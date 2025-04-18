using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.DTOs;

namespace ContentService.Interfaces
{
    public interface IExamPracticeManager
    {
        Task<string?> CreateExamPracticeAsync(CreateExamRequest request);
        ComponentResponse CreateExamComponent(CreateExamComponentRequest request);
        ExamResponse GetExamPracticeById(int id);
        ExamResponse GetAllExamPractices();
        bool UpdateExamPractice(UpdateExamRequest request);
        bool DeleteExamPractice(int id);
        Task StartListeningAsync();
    }
}
