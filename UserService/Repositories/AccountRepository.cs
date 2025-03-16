using Microsoft.Azure.Cosmos;
using UserService.Domain;
using UserService.Helpers;

namespace UserService.Repositories
{
    public class AccountRepository
    {
        private readonly Container _container;

        // Constructor to initialize the repository with a container
        public AccountRepository(string databaseName, string containerName)
        {
            // Use CosmosDbConnection to get the container
            _container = CosmosDBConnection.GetContainerAsync(databaseName, containerName).Result;
        }

        // Method to save a user to Cosmos DB
        public async Task SaveTeacherAccountAsync(TeacherAccount account)
        {
            try
            {
                await _container.CreateItemAsync(account, new PartitionKey(account.Id));
                Console.WriteLine($"User {account.FirstName} saved successfully.");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error saving user: {ex.Message}");
            }
        }
    }
}
