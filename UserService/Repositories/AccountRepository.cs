using Microsoft.Azure.Cosmos;
using UserService.Domain;
using UserService.Helpers;
using UserService.Interfaces;

namespace UserService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Container _container;

        // Constructor to initialize the repository with a container
        public AccountRepository(CosmosDBConnection cosmosDBConnection, string databaseName, string containerName)
        {
            // Use CosmosDBConnection to get the container asynchronously
            _container = cosmosDBConnection.GetContainerAsync(databaseName, containerName).Result;
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
