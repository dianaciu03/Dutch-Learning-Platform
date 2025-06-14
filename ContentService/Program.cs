using System.Text.Json;
using ContentService.Helpers;
using ContentService.Interfaces;
using ContentService.Managers;
using ContentService.Repositories;
using DotNetEnv;
using Newtonsoft.Json;
using Prometheus;
using Serilog;

namespace ContentService
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
                Log.Information("Starting Content Service...");
                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog to the builder
                builder.Host.UseSerilog();

                // Check if the environment is Docker (from GitHub CI/CD pipeline)
                var dockerEnv = Environment.GetEnvironmentVariable("DOCKER_ENV");
                var integrationTestEnv = Environment.GetEnvironmentVariable("INTEGRATION_TEST_ENV");
                var kubernetesEnv = Environment.GetEnvironmentVariable("KUBERNETES_ENV");

                Log.Information("DOCKER_ENV: {DockerEnv} or KUBERNETES_ENV: {KubernetesEnv}", dockerEnv, kubernetesEnv);

                var envPrefix = (dockerEnv == "true") ? "DOCKER_" : (integrationTestEnv == "true") ? "INT_TEST_" : "";

                if (dockerEnv != "true")
                {
                    // LOCAL ENVIRONMENT
                    var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
                    Env.Load(envFilePath);
                }

                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    });

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Register RabbitMQConnection as a Singleton
                builder.Services.AddSingleton<RabbitMQConnection>(sp =>
                {
                    string hostName = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_HOST") ?? "localhost";
                    string userName = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_USERNAME") ?? "guest";
                    string password = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_PASSWORD") ?? "guest";
                    return new RabbitMQConnection(hostName, userName, password);
                });

                builder.Services.AddHostedService<RabbitMQListener>();

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

                // Register AccountRepository as implementation for IAccountRepository
                builder.Services.AddScoped<IExamPracticeRepository, ExamPracticeRepository>(sp =>
                {
                    // Resolve CosmosDBConnection from DI container
                    var cosmosDBConnection = sp.GetRequiredService<CosmosDBConnection>();

                    string databaseName = Environment.GetEnvironmentVariable($"{envPrefix}COSMOSDB_DATABASE_NAME");
                    string containerName = Environment.GetEnvironmentVariable($"{envPrefix}COSMOSDB_CONTAINER_NAME_CONTENT_SERVICE");

                    if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(containerName))
                    {
                        throw new InvalidOperationException("Cosmos DB database or container name is not set.");
                    }

                    // Instantiate and return AccountRepository with dynamic database and container names
                    return new ExamPracticeRepository(cosmosDBConnection, databaseName, containerName);
                });

                // Add services to the container
                builder.Services.AddScoped<IExamPracticeManager, ExamPracticeManager>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Enable Prometheus scraping at /metrics
                app.UseRouting();
                app.UseHttpMetrics(); // middleware that tracks request durations, status codes, etc.
                app.MapMetrics();  // This exposes metrics at the /metrics endpoint

                app.UseHttpsRedirection();
                app.UseAuthorization();
                
                if (dockerEnv == "true")
                {
                    // DOCKER ONLY
                    app.Urls.Add("http://0.0.0.0:8082");
                    //app.Urls.Add("https://0.0.0.0:8083");
                }

                // Add a health endpoint for Kubernetes liveness/readiness probes
                app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Content Service terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
