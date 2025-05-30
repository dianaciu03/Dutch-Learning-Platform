using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Serilog;

namespace ContentService.Helpers
{
    public class CosmosDBConnection
    {
        private CosmosClient cosmosClient;
        private readonly string _connectionString;

        public CosmosDBConnection(string connectionString)
        {
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
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(30), // Wait longer before retrying,
                UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
                {
                    Converters = { new ExamComponentConverter() }
                }
            };

            // Initialize the Cosmos client
            cosmosClient = new CosmosClient(_connectionString, clientOptions: options);
            Log.Information("CosmosDB Client instantiated successfully");
        }

        // Method to get a Cosmos database (Create if not exists)
        public async Task<Database> GetDatabaseAsync(string databaseName)
        {
            // Check if the database exists; create it if not
            try
            {
                Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
                Log.Information("Created or retrieved database {DatabaseName}", databaseName);
                return database;
            }
            catch (CosmosException ex)
            {
                Log.Error(ex, "Error while creating/getting the database {DatabaseName}", databaseName);
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
                Log.Information("Created or retrieved container {ContainerName} in database {DatabaseName}", containerName, databaseName);
                return container;
            }
            catch (CosmosException ex)
            {
                Log.Error(ex, "Error while creating/getting the container {ContainerName} in database {DatabaseName}", containerName, databaseName);
                throw;
            }
        }
    }
}
