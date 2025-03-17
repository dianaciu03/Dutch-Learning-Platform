using Microsoft.Azure.Cosmos;

namespace UserService.Helpers
{
    public class CosmosDBConnection
    {
        private CosmosClient cosmosClient;
        private readonly string _connectionString;
        private readonly LogHelper<CosmosDBConnection> _logger;

        public CosmosDBConnection(string connectionString)
        {
            _logger = new LogHelper<CosmosDBConnection>();
            // Get the connection string from environment variables
            _connectionString = connectionString;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string is missing.");
            }

            // Initialize the Cosmos client
            cosmosClient = new CosmosClient(_connectionString);
            _logger.LogInfo("CosmosDB Client instantiated successfully.");
        }

        // Method to get a Cosmos database (Create if not exists)
        public async Task<Database> GetDatabaseAsync(string databaseName)
        {
            // Check if the database exists; create it if not
            try
            {
                //TODO: This method executes in a loop, check issues
                Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
                return database;
            }
            catch (CosmosException ex)
            {
                _logger.LogError("Error while creating/getting the database.", ex);
                throw;
            }
        }

        // Method to get a Cosmos container (Create if not exists)
        public async Task<Container> GetContainerAsync(string databaseName, string containerName, string partitionKeyPath = "/id")
        {
            try
            {
                // First, ensure the database exists
                Database database = await GetDatabaseAsync(databaseName);

                // Now, create the container if it does not exist
                Container container = await database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath);
                return container;
            }
            catch (CosmosException ex)
            {
                _logger.LogError("Error while creating/getting the container.", ex);
                throw;
            }
        }
    }
}
