using ContentService.Domain;
using ContentService.Domain.ExamComponents;
using ContentService.Helpers;
using ContentService.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentService.Repositories
{
    public class ExamPracticeRepository : IExamPracticeRepository
    {
        private readonly Container _container;

        // Constructor to initialize the repository with a container
        public ExamPracticeRepository(CosmosDBConnection cosmosDBConnection, string databaseName, string containerName)
        {
            // Use CosmosDBConnection to get the container asynchronously
            _container = cosmosDBConnection.GetContainerAsync(databaseName, containerName).Result;
        }

        public async Task<string?> SaveExamPracticeAsync(ExamPractice exam)
        {
            try
            {
                // Use existing id or generate a new one if not provided
                if (string.IsNullOrWhiteSpace(exam.id))
                {
                    exam.id = Guid.NewGuid().ToString();
                }

                // Upsert (create or update)
                var response = await _container.UpsertItemAsync(exam, new PartitionKey(exam.id));

                Console.WriteLine($"Exam with id {exam.id} was upserted successfully. Status code: {response.StatusCode}");
                return exam.id;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error upserting exam practice: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> SaveExamComponentAsync(IExamComponent component)
        {
            try
            {
                var componentType = component.GetComponentType().ToString();

                switch (componentType)
                {
                    case "Reading":
                        component = component as ReadingComponent ?? throw new InvalidCastException("Invalid ReadingComponent.");
                        break;
                    // Add more component types here as needed.
                    default:
                        throw new NotSupportedException($"Unsupported component type: {componentType}");
                }

                if (string.IsNullOrWhiteSpace(component.ExamId))
                {
                    Console.WriteLine("Component is missing ExamId.");
                    return null;
                }

                var exam = await GetExamPracticeByIdAsync(component.ExamId);
                if (exam == null)
                {
                    Console.WriteLine($"No exam found with ID: {component.ExamId}");
                    return null;
                }

                // Add the new component
                exam.ExamComponents.Add(component);

                // Save updated exam using the centralized method
                var savedId = await SaveExamPracticeAsync(exam);

                return savedId == null ? null : component.id;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error saving exam component: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        public async Task<ExamPractice?> GetExamPracticeByIdAsync(string examId)
        {
            try
            {
                var response = await _container.ReadItemAsync<JObject>(examId, new PartitionKey(examId));
                JObject raw = response.Resource;

                // Deserialize manually with your settings
                var settings = new JsonSerializerSettings
                {
                    Converters = { new ExamComponentConverter() },
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.None
                };

                var exam = raw.ToObject<ExamPractice>(JsonSerializer.Create(settings));
                return exam;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error fetching exam with ID {examId}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }
    }
}
