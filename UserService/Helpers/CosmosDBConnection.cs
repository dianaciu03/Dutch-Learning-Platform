using Microsoft.Azure.Cosmos;

namespace UserService.Helpers
{
    public class CosmosDBConnection
    {
        private static CosmosClient cosmosClient;
        private static string connectionString;
        private static readonly LogHelper<CosmosDBConnection> _logger;

        // Static constructor to initialize the Cosmos client
        static CosmosDBConnection()
        {
            _logger = new LogHelper<CosmosDBConnection>();

            // Get the connection string from environment variables
            connectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is missing.");
            }

            // Initialize the Cosmos client
            cosmosClient = new CosmosClient(connectionString);
        }

        // Method to get a Cosmos database (Create if not exists)
        public static async Task<Database> GetDatabaseAsync(string databaseName)
        {
            // Check if the database exists; create it if not
            try
            {
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
        public static async Task<Container> GetContainerAsync(string databaseName, string containerName, string partitionKeyPath = "/id")
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
