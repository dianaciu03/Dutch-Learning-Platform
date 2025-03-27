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
                var id = Guid.NewGuid().ToString();
                account.id = id;
                await _container.CreateItemAsync(account, new PartitionKey(account.id));
                Console.WriteLine($"User {account.FirstName} saved successfully.");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error saving user: {ex.Message}");
            }
        }

        // Save Student Account
        public async Task SaveStudentAccountAsync(StudentAccount account)
        {
            try
            {
                account.id = Guid.NewGuid().ToString();
                await _container.CreateItemAsync(account, new PartitionKey(account.id));
                Console.WriteLine($"Student {account.FirstName} saved successfully.");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error saving student: {ex.Message}");
            }
        }

        // Get Account By ID
        public async Task<TeacherAccount> GetTeacherAccountByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<TeacherAccount>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Account with ID {id} not found.");
                return null;
            }
        }

        public async Task<StudentAccount> GetStudentAccountByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<StudentAccount>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Account with ID {id} not found.");
                return null;
            }
        }

        // Get All Accounts
        public async Task<List<UserAccount>> GetAllAccountsAsync()
        {
            try
            {
                var query = _container.GetItemQueryIterator<UserAccount>("SELECT * FROM c");
                var results = new List<UserAccount>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response);
                }

                return results;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error fetching all accounts: {ex.Message}");
                return new List<UserAccount>();
            }
        }

        // Delete Account
        public async Task<bool> DeleteAccountAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<UserAccount>(id, new PartitionKey(id));
                Console.WriteLine($"Account {id} deleted successfully.");
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Account with ID {id} not found.");
                return false;
            }
        }
    }
}
