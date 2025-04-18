using ContentService.Domain;
using ContentService.Helpers;
using ContentService.Interfaces;
using Microsoft.Azure.Cosmos;

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
        }
    }
}
