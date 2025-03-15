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
        private readonly LogHelper<ExamPractice> _logger;

        public ExamPracticeManager(LogHelper<ExamPractice> logHelper)
        {
            _logger = logHelper;
        }

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
                _logger.LogError("Error while processing exam.", ex);
                return false;
            }
        }

        public ComponentResponse CreateExamComponent(CreateExamComponentRequest request)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError("Error while creating exam component.", ex);
                return new ComponentResponse { Component = null };
            }


        }

        // Read
        public ExamResponse GetExamPracticeById(int id)
        {
            try
            {
                _logger.LogInfo("Fetching exam with ID: {0}", id);
                var exam = _exams.FirstOrDefault(e => e.Id == id);
                return new ExamResponse { ExamList = new List<ExamPractice> { exam } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching exam.", ex);
                return new ExamResponse { ExamList = new List<ExamPractice>() };
            }

        }

        public ExamResponse GetAllExamPractices()
        {
            try
            {
                return new ExamResponse { ExamList = _exams };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting all exams.", ex);
                return new ExamResponse { ExamList = new List<ExamPractice>() };
            }
        }

        // Update
        public bool UpdateExamPractice(UpdateExamRequest updateExam)
        {
            try
            {
                var exam = _exams.FirstOrDefault(e => e.Id == updateExam.Id);
                if (exam == null)
                {
                    return false;
                }
                exam.ExamTypes = updateExam.ExamTypes.Select(type => EnumConverter.ParseExamType(type)).ToList();
                exam.Level = EnumConverter.ParseCEFRLevel(updateExam.Level);
                exam.ExamComponents = updateExam.ExamComponents;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while updating exam.", ex);
                return false;
            }
        }

        // Delete
        public bool DeleteExamPractice(int id)
        {
            try
            {
                var exam = _exams.FirstOrDefault(e => e.Id == id);
                if (exam == null)
                {
                    _logger.LogWarning("Exam could not be found.");
                    return false;
                }
                _exams.Remove(exam);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting exam.", ex);
                return false;
            }
        }
    }
}
