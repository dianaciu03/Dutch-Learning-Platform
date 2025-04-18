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

        public async Task SaveExamPracticeAsync(ExamPractice exam)
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                exam.id = id;
                await _container.CreateItemAsync(exam, new PartitionKey(exam.id));
                Console.WriteLine($"The exam with id {exam.id} was saved successfully.");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error saving exam practice: {ex.Message}");
            }
        }
    }
}
