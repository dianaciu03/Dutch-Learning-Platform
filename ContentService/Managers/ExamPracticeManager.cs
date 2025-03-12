using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.DTOs;
using ContentService.Domain.ExamComponents;
using ContentService.Helpers;
using ContentService.Interfaces;

namespace ContentService.Managers
{
    public class ExamPracticeManager : IExamPracticeManager
    {
        private readonly List<ExamPractice> _exams = [];

        // Create
        public bool CreateExamPractice(CreateExamRequest request)
        {
            try
            {
                var examTypes = request.ExamTypes.Select(type => EnumConverter.ParseExamType(type)).ToList();
                var level = EnumConverter.ParseCEFRLevel(request.Level);
                var exercise = new ExamPractice(examTypes, level, request.MaxPoints, request.ExamComponents);
                _exams.Add(exercise);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public ComponentResponse CreateExamComponent(CreateExamComponentRequest request)
        {
            ExamType type = EnumConverter.ParseExamType(request.ComponentType);

            IExamComponent component = type switch
            {
                ExamType.Reading => new ReadingComponent(),
                ExamType.Vocabulary => new VocabularyComponent(),
                ExamType.Listening => new ListeningComponent(),
                ExamType.Writing => new WritingComponent(),
                ExamType.Speaking => new SpeakingComponent(),
                ExamType.Grammar => new GrammarComponent(),
                _ => throw new ArgumentException($"Unsupported exam type: {request.ComponentType}")
            };

            return new ComponentResponse { Component = component };
        }

        // Read
        public ExamResponse GetExamPracticeById(int id)
        {
            var exam = _exams.FirstOrDefault(e => e.Id == id);

            return new ExamResponse
            {
                ExamList = new List<ExamPractice> { exam }
            };
        }

        public ExamResponse GetAllExamPractices()
        {
            return new ExamResponse
            {
                ExamList = _exams.ToList()
            };
        }

        // Update
        public bool UpdateExamPractice(UpdateExamRequest updateExam)
        {
            var exam = _exams.FirstOrDefault(e => e.Id == updateExam.Id);

            if (exam == null)
            {
                return false;
            }

            exam.Level = EnumConverter.ParseCEFRLevel(updateExam.Level);
            exam.ExamTypes = updateExam.ExamTypes.Select(type => EnumConverter.ParseExamType(type)).ToList();
            exam.ExamComponents = updateExam.ExamComponents;

            return true;
        }

        // Delete
        public bool DeleteExamPractice(int id)
        {
            var exam = _exams.FirstOrDefault(e => e.Id == id);

            if (exam == null)
            {
                return false;
            }

            _exams.Remove(exam);
            return true;
        }
    }
}
