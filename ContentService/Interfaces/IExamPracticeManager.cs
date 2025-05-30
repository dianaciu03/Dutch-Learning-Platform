﻿using System;
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
        Task<string?> CreateReadingComponentAsync(CreateReadingComponentRequest request);
        ExamResponse GetExamPracticeById(string id);
        ExamResponse GetAllExamPractices();
        bool UpdateExamPractice(UpdateExamRequest request);
        bool DeleteExamPracticeById(string id);
        int DeleteAllExamPractices();
    }
}
