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
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace ContentService.Managers
{
    public class ExamPracticeManager : IExamPracticeManager
    {
        private readonly LogHelper<ExamPracticeManager> _logger;
        private readonly IExamPracticeRepository _examPracticeRepository;

        public ExamPracticeManager(IExamPracticeRepository examPracticeRepository)
        {
            _logger = new LogHelper<ExamPracticeManager>();
            _examPracticeRepository = examPracticeRepository;
        }

        // Create
        public async Task<string?> CreateExamPracticeAsync(CreateExamRequest request)
        {
            try
            {
                var level = EnumConverter.ParseCEFRLevel(request.Level);

                if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 100)
                {
                    _logger.LogWarning("Invalid exam name provided.");
                    return null;
                }

                if (request.MaxPoints <= 0 || request.MaxPoints > 1000)
                {
                    _logger.LogWarning("Inavlid number of points provided. The value should be between 0 and 1000.");
                    return null;
                }

                // Create the exam object
                var examPractice = new ExamPractice(request.Name, level, request.MaxPoints);

                // If request includes an ID, assign it
                if (!string.IsNullOrWhiteSpace(request.id))
                {
                    examPractice.id = request.id;
                }

                var id = await _examPracticeRepository.SaveExamPracticeAsync(examPractice);

                if (id == null)
                {
                    _logger.LogWarning("Exam could not be created.");
                    return null;
                }

                _logger.LogInfo("Empty exam was updated: {0}", examPractice.Name);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing exam.", ex);
                return null;
            }
        }

        public async Task<string?> CreateReadingComponentAsync(CreateReadingComponentRequest request)
        {
            try
            {
                // Create the reading component from the request
                IExamComponent readingComponent = new ReadingComponent(request.ExamId, request.Text, request.Questions);

                // If request includes an ID, assign it
                if (!string.IsNullOrWhiteSpace(request.id))
                {
                    readingComponent.id = request.id;
                }

                // Save using your repository method
                var savedId = await _examPracticeRepository.SaveExamComponentAsync(readingComponent);

                if (savedId == null)
                {
                    _logger.LogWarning("Reading component could not be created or updated.");
                    return null;
                }

                _logger.LogInfo("Reading component was created or updated: {0}", savedId);
                return savedId;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing reading component.", ex);
                return null;
            }
        }

        // Read
        public ExamResponse GetExamPracticeById(string id)
        {
            try
            {
                _logger.LogInfo("Fetching exam with ID: {0}", id);
                var examPractice = _examPracticeRepository.GetExamPracticeByIdAsync(id).Result;

                if (examPractice == null)
                {
                    return new ExamResponse { ExamList = new List<ExamPractice>() };
                }

                return new ExamResponse { ExamList = new List<ExamPractice> { examPractice } };
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
                var examPractices = _examPracticeRepository.GetAllExamPracticesAsync().Result; // Await the task to get the result  
                return new ExamResponse { ExamList = examPractices }; // Map the result to ExamResponse  
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
                var exam = 0; //= _exams.FirstOrDefault(e => e.id == updateExam.Id);
                if (exam == null)
                {
                    return false;
                }
                //exam.ExamTypes = updateExam.ExamTypes.Select(type => EnumConverter.ParseExamType(type)).ToList();
                //exam.Level = EnumConverter.ParseCEFRLevel(updateExam.Level);
                //exam.ExamComponents = updateExam.ExamComponents;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while updating exam.", ex);
                return false;
            }
        }

        // Delete
        public bool DeleteExamPracticeById(string id)
        {
            try
            {
                _logger.LogInfo("Deleting exam with ID: {0}", id);

                var success = _examPracticeRepository.DeleteExamPracticeByIdAsync(id).Result;

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting exam.", ex);
                return false;
            }
        }

        public int DeleteAllExamPractices()
        {
            try
            {
                _logger.LogInfo("Deleting all exams.");

                var deletedCount = _examPracticeRepository.DeleteAllExamPracticesAsync().Result;

                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting all exams.", ex);
                return -1;
            }
        }
    }
}
