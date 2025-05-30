using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.DTOs;
using ContentService.Domain.ExamComponents;
using ContentService.Interfaces;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Serilog;
using ContentService.Helpers;

namespace ContentService.Managers
{
    public class ExamPracticeManager : IExamPracticeManager
    {
        private readonly IExamPracticeRepository _examPracticeRepository;

        public ExamPracticeManager(IExamPracticeRepository examPracticeRepository)
        {
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
                    Log.Warning("Invalid exam name provided.");
                    return null;
                }

                if (request.MaxPoints <= 0 || request.MaxPoints > 1000)
                {
                    Log.Warning("Invalid number of points provided. The value should be between 0 and 1000.");
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
                    Log.Warning("Exam could not be created.");
                    return null;
                }

                Log.Information("Empty exam was updated: {ExamName}", examPractice.Name);
                return id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while processing exam");
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
                    Log.Warning("Reading component could not be created or updated.");
                    return null;
                }

                Log.Information("Reading component was created or updated: {ComponentId}", savedId);
                return savedId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while processing reading component");
                return null;
            }
        }

        // Read
        public ExamResponse GetExamPracticeById(string id)
        {
            try
            {
                Log.Information("Fetching exam with ID: {ExamId}", id);
                var examPractice = _examPracticeRepository.GetExamPracticeByIdAsync(id).Result;

                if (examPractice == null)
                {
                    return new ExamResponse { ExamList = new List<ExamPractice>() };
                }

                return new ExamResponse { ExamList = new List<ExamPractice> { examPractice } };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while fetching exam");
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
                Log.Error(ex, "Error while getting all exams");
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
                Log.Error(ex, "Error while updating exam");
                return false;
            }
        }

        // Delete
        public bool DeleteExamPracticeById(string id)
        {
            try
            {
                Log.Information("Deleting exam with ID: {ExamId}", id);

                var success = _examPracticeRepository.DeleteExamPracticeByIdAsync(id).Result;

                return success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while deleting exam");
                return false;
            }
        }

        public int DeleteAllExamPractices()
        {
            try
            {
                Log.Information("Deleting all exams");

                var deletedCount = _examPracticeRepository.DeleteAllExamPracticesAsync().Result;

                return deletedCount;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while deleting all exams");
                return -1;
            }
        }
    }
}
