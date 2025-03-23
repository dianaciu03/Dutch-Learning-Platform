using DotNetEnv;
using UserService.Helpers;
using UserService.Interfaces;
using UserService.Managers;
using UserService.Repositories;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLogging();

            // Check if the environment is Docker (from GitHub CI/CD pipeline)
            var dockerEnv = Environment.GetEnvironmentVariable("DOCKER_ENV");
            Console.WriteLine($"DOCKER_ENV: {dockerEnv}");


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
            builder.Services.AddScoped(typeof(LogHelper<>));
            builder.Services.AddScoped<IAccountManager, AccountManager>();

            // Register CosmosDBConnection as a Singleton
            builder.Services.AddSingleton<CosmosDBConnection>(sp =>
            {
                string connectionString;

                if (dockerEnv == "true")
                {
                    // DOCKER ENVIRONMENT
                    connectionString = Environment.GetEnvironmentVariable("DOCKER_COSMOSDB_CONNECTION_STRING");
                }
                else
                {
                    // LOCAL ENVIRONMENT
                    connectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
                }

                // Check if the connection string is empty or null
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Cosmos DB connection string is not set.");
                }

                // Return the CosmosDB connection with the appropriate connection string
                return new CosmosDBConnection(connectionString);
            });


            // Register AccountRepository as implementation for IAccountRepository
            builder.Services.AddScoped<IAccountRepository, AccountRepository>(sp =>
            {
                // Resolve CosmosDBConnection from DI container
                var cosmosDBConnection = sp.GetRequiredService<CosmosDBConnection>();

                string databaseName;
                string containerName;

                if (dockerEnv == "true")
                {
                    // DOCKER ENVIRONMENT
                    databaseName = Environment.GetEnvironmentVariable("DOCKER_COSMOSDB_DATABASE_NAME"); 
                    containerName = Environment.GetEnvironmentVariable("DOCKER_COSMOSDB_CONTAINER_NAME"); 
                }
                else
                {
                    // LOCAL ENVIRONMENT
                    databaseName = Environment.GetEnvironmentVariable("COSMOSDB_DATABASE_NAME"); 
                    containerName = Environment.GetEnvironmentVariable("COSMOSDB_CONTAINER_NAME"); 
                }

                if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(containerName))
                {
                    throw new InvalidOperationException("Cosmos DB database or container name is not set.");
                }

                // Instantiate and return AccountRepository with dynamic database and container names
                return new AccountRepository(cosmosDBConnection, databaseName, containerName);
            });


            // Register RabbitMQConnection as a Singleton
            builder.Services.AddSingleton<RabbitMQConnection>(sp =>
            {
                // Check if we are running in a Docker environment (based on the DOCKER_ENV variable)
                var dockerEnv = Environment.GetEnvironmentVariable("DOCKER_ENV");

                string rabbitMqHost, rabbitMqUser, rabbitMqPassword;

                if (dockerEnv == "true")
                {
                    // DOCKER ENVIRONMENT
                    rabbitMqHost = Environment.GetEnvironmentVariable("DOCKER_RABBITMQ_HOST");
                    rabbitMqUser = Environment.GetEnvironmentVariable("DOCKER_RABBITMQ_USER");
                    rabbitMqPassword = Environment.GetEnvironmentVariable("DOCKER_RABBITMQ_PASSWORD");
                }
                else
                {
                    // LOCAL ENVIRONMENT
                    rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                    rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER");
                    rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
                }

                if (string.IsNullOrEmpty(rabbitMqHost) || string.IsNullOrEmpty(rabbitMqUser) || string.IsNullOrEmpty(rabbitMqPassword))
                {
                    throw new InvalidOperationException("RabbitMQ connection details are not set.");
                }

                // Return the RabbitMQ connection using the values fetched from the environment variables
                return new RabbitMQConnection(rabbitMqHost, rabbitMqUser, rabbitMqPassword);
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseAuthorization();

            if (dockerEnv == "true")
            {
                // DOCKER ONLY
                app.Urls.Add("http://0.0.0.0:8084");
                //app.Urls.Add("https://0.0.0.0:8085");
            }

            app.MapControllers();

            app.Run();
        }
    }
}
