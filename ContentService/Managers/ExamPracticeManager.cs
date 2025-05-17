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
        private readonly RabbitMQConnection _rabbitMqConnection;
        private readonly List<ExamPractice> _exams = [];
        private readonly LogHelper<ExamPracticeManager> _logger;

        public ExamPracticeManager(RabbitMQConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection;
            _logger = new LogHelper<ExamPracticeManager>();
        }

        // Create
        public bool CreateExamPractice(CreateExamRequest request)
        {
            try
            {
                var examTypes = request.ExamTypes.Select(type => EnumConverter.ParseExamType(type)).ToList();
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

        public async Task StartListeningAsync()
        {
            var channel = _rabbitMqConnection.GetChannel();
            _logger.LogInfo("Listening to the channel {0}", channel);

            // Declare the queue you want to listen to
            await channel.QueueDeclareAsync(queue: "accountQueue", durable: true, exclusive: false, autoDelete: false);
            _logger.LogInfo("Listening to the accountQueue...");

            // Create a consumer
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Extract account id from the message
                var accountId = ExtractAccountId(message);

                if (accountId.HasValue)
                {
                    // Fetch the user data (e.g., GetAllExamsByUserId)
                    //var content = await _accountRepository.GetAllExamsByUserId(accountId.Value);
                    _logger.LogInfo("Received the message and it contains the id: {0}", accountId);
                }
            };

            // Start listening to the queue
            await channel.BasicConsumeAsync(queue: "accountQueue", autoAck: true, consumer: consumer);

            //_logger.LogInfo("Content service is listening for messages...");
        }

        private Guid? ExtractAccountId(string message)
        {
            // Assuming the message follows this format: "Account to delete: {id}"
            if (message.StartsWith("Account to delete:"))
            {
                var parts = message.Split(':');
                if (parts.Length > 1)
                {
                    string idPart = parts[1].Trim();
                    if (Guid.TryParse(idPart, out Guid id))
                    {
                        return id;
                    }
                }
            }
            return null;
        }
    }
}
