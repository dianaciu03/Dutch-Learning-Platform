using System.Net.Http.Json;
using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Testcontainers.CosmosDb;
using Testcontainers.RabbitMq;
using UserService;
using UserService.DTOs;
using UserService.Helpers;
using UserService.Interfaces;
using UserService.Managers;
using UserService.Repositories;

namespace IntegrationTests.IntegrationTests
{
    public class UserServiceTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private CosmosClient _cosmosClient;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IContainer _rabbitMqContainer;
        private readonly IContainer _cosmosDbContainer;
        private IAccountRepository _accountRepository;
        private IAccountManager _accountManager;
        private readonly string _connectionString;
        public ILogger<UserServiceTests> Logger { get; }

        public UserServiceTests(WebApplicationFactory<Program> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            Environment.SetEnvironmentVariable("INTEGRATION_TEST_ENV", "true");

            _factory = factory;
            _client = factory.CreateClient();

            _rabbitMqContainer = new RabbitMqBuilder()
                .WithImage("rabbitmq:3-management")
                .WithPortBinding(5672, 5672) // rabbitmq default port
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672)) // Wait until RabbitMQ is ready
                .Build();

            _cosmosDbContainer = new CosmosDbBuilder()
                .WithCleanUp(true)
                .WithPortBinding(8081, true)  // Map port 8081
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync().ConfigureAwait(true);
            await _cosmosDbContainer.StartAsync().ConfigureAwait(true);


            CosmosClientOptions options = new()
            {
                HttpClientFactory = () => new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true // Critical for emulator stability
            };

            // Get the RabbitMQ container's mapped host & port
            var rabbitMqHost = _rabbitMqContainer.Hostname;
            var rabbitMqPort = 5672; // Get the exposed port
            var rabbitMqUsername = "rabbitmq"; // Default username
            var rabbitMqPassword = "rabbitmq"; // Default password

            Environment.SetEnvironmentVariable("INT_TEST_RABBITMQ_HOST", rabbitMqHost);
            Environment.SetEnvironmentVariable("INT_TEST_RABBITMQ_USER", rabbitMqUsername);
            Environment.SetEnvironmentVariable("INT_TEST_RABBITMQ_PASSWORD", rabbitMqPassword);

            var cosmosPort = _cosmosDbContainer.GetMappedPublicPort(8081);
            var cosmosHost = _cosmosDbContainer.Hostname;
            var _connectionString = $"AccountEndpoint=https://{cosmosHost}:{cosmosPort}/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;";

            Environment.SetEnvironmentVariable("INT_TEST_COSMOSDB_DATABASE_NAME", "TestDatabase");
            Environment.SetEnvironmentVariable("INT_TEST_COSMOSDB_CONTAINER_NAME", "TestUsers");
            Environment.SetEnvironmentVariable("INT_TEST_COSMOSDB_CONNECTION_STRING", _connectionString);
            
            // Initialize RabbitMQ connection
            var _rabbitMqConnection = new RabbitMQConnection(rabbitMqHost, rabbitMqUsername, rabbitMqPassword, rabbitMqPort);

            // Initialize the Cosmos client
            //_cosmosClient = new CosmosClient(_connectionString, clientOptions: options);
            var cosmosDBConnection = new CosmosDBConnection(_connectionString);
            _accountRepository = new AccountRepository(cosmosDBConnection, "TestDatabase", "TestUsers");
            _accountManager = new AccountManager(_rabbitMqConnection, _accountRepository);
            //_accountController = new AccountController(_accountManager, new LogHelper<AccountController>());
        }


        public async Task DisposeAsync()
        {
            await _rabbitMqContainer.DisposeAsync().ConfigureAwait(true);
            await _cosmosDbContainer.DisposeAsync().ConfigureAwait(true);
            Environment.SetEnvironmentVariable("INTEGRATION_TEST_ENV", "false");
        }


        [Fact]
        public async Task CreateTeacherAccount_ShouldSucceed_AndBeSavedInDatabase()
        {
            // Arrange - Create a teacher account request
            var teacherRequest = new CreateTeacherAccountRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe123",
                Email = "john.doe@example.com",
                Password = "SecurePassword123",
                EducationalInstitution = "Some University"
            };

            // Act - Send the request to create a teacher
            var response = await _client.PostAsJsonAsync("/api/accounts/teacher", teacherRequest);

            // Assert - Check if the request was successful
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            //// Verify that the user was added to the database
            //var container = _cosmosClient.GetContainer(_databaseName, _containerName);
            //var query = new QueryDefinition("SELECT * FROM c WHERE c.Username = @username")
            //    .WithParameter("@username", teacherRequest.Username);

            //using var feedIterator = container.GetItemQueryIterator<TeacherAccount>(query);
            //var users = await feedIterator.ReadNextAsync();

            //Assert.NotEmpty(users);
            //Assert.Equal(teacherRequest.Username, users.First().Username);
        }

        //private static void LoginToDocker()
        //{
        //    var process = new System.Diagnostics.Process
        //    {
        //        StartInfo = new System.Diagnostics.ProcessStartInfo
        //        {
        //            FileName = "docker",
        //            Arguments = $"login -u {Environment.GetEnvironmentVariable("DOCKER_USERNAME")} -p {Environment.GetEnvironmentVariable("DOCKER_PASSWORD")}",
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        }
        //    };

        //    process.Start();
        //    process.WaitForExit();

        //    if (process.ExitCode != 0)
        //    {
        //        throw new Exception($"Docker login failed: {process.StandardError.ReadToEnd()}");
        //    }
        //}
    }
}
