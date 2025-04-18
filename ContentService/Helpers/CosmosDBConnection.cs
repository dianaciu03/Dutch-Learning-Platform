using Microsoft.Azure.Cosmos;

namespace ContentService.Helpers
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

            CosmosClientOptions options = new()
            {
                HttpClientFactory = () => new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true, // Critical for emulator stability
                RequestTimeout = TimeSpan.FromMinutes(1), // Increase the timeout
                MaxRetryAttemptsOnRateLimitedRequests = 10, // Retry on rate-limited requests
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(30) // Wait longer before retrying
            };

            // Initialize the Cosmos client
            cosmosClient = new CosmosClient(_connectionString, clientOptions: options);
            _logger.LogInfo("CosmosDB Client instantiated successfully.");
        }

        // Method to get a Cosmos database (Create if not exists)
        public async Task<Database> GetDatabaseAsync(string databaseName)
        {
            // Check if the database exists; create it if not
            try
            {
                Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
                _logger.LogInfo("Created or retrieved database {0}", database);
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
