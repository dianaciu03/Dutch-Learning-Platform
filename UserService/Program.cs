using DotNetEnv;
using Prometheus;
using UserService.Helpers;
using UserService.Interfaces;
using UserService.Managers;
using UserService.Repositories;
using Serilog;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting User Service...");
                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog to the builder
                builder.Host.UseSerilog();

                // Check if the environment is Docker (from GitHub CI/CD pipeline)
                var dockerEnv = Environment.GetEnvironmentVariable("DOCKER_ENV");
                var integrationTestEnv = Environment.GetEnvironmentVariable("INTEGRATION_TEST_ENV");
                Log.Information("DOCKER_ENV: {DockerEnv}", dockerEnv);

                var envPrefix = (dockerEnv == "true") ? "DOCKER_" : (integrationTestEnv == "true") ? "INT_TEST_" : "";

                if (dockerEnv != "true")
                {
                    // LOCAL ENVIRONMENT
                    var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
                    Env.Load(envFilePath);
                }

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Add services to the container.
                builder.Services.AddScoped<IAccountManager, AccountManager>();

                // Register CosmosDBConnection as a Singleton
                builder.Services.AddSingleton<CosmosDBConnection>(sp =>
                {
                    string connectionString = Environment.GetEnvironmentVariable($"{envPrefix}COSMOSDB_CONNECTION_STRING");
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new InvalidOperationException("Cosmos DB connection string is not set.");
                    }
                    return new CosmosDBConnection(connectionString);
                });

                // Register RabbitMQConnection as a Singleton
                builder.Services.AddSingleton<RabbitMQConnection>(sp =>
                {
                    string hostName = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_HOST") ?? "localhost";
                    string userName = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_USERNAME") ?? "guest";
                    string password = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_PASSWORD") ?? "guest";

                    return new RabbitMQConnection(hostName, userName, password);
                });

                // Register AccountRepository as implementation for IAccountRepository
                builder.Services.AddScoped<IAccountRepository, AccountRepository>(sp =>
                {
                    // Resolve CosmosDBConnection from DI container
                    var cosmosDBConnection = sp.GetRequiredService<CosmosDBConnection>();

                    string databaseName = Environment.GetEnvironmentVariable($"{envPrefix}COSMOSDB_DATABASE_NAME");
                    string containerName = Environment.GetEnvironmentVariable($"{envPrefix}COSMOSDB_CONTAINER_NAME_USER_SERVICE");

                    if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(containerName))
                    {
                        throw new InvalidOperationException("Cosmos DB database or container name is not set.");
                    }

                    // Instantiate and return AccountRepository with dynamic database and container names
                    return new AccountRepository(cosmosDBConnection, databaseName, containerName);
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Enable Prometheus scraping at /metrics
                app.UseRouting();
                app.UseHttpMetrics();
                app.MapMetrics();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "User Service terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
