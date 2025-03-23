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

            var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
            Env.Load(envFilePath); 

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();

            // Add services to the container.
            builder.Services.AddScoped(typeof(LogHelper<>));
            builder.Services.AddScoped<IAccountManager, AccountManager>();

            // Register AccountRepository as implementation for IAccountRepository
            builder.Services.AddScoped<IAccountRepository, AccountRepository>(sp =>
            {
                // Resolve CosmosDBConnection from DI container
                var cosmosDBConnection = sp.GetRequiredService<CosmosDBConnection>();

                // Hardcoded values for testing
                var databaseName = "SlimStudie";  // Hardcode your database name
                var containerName = "UserAccounts"; // Hardcode your container name

                // Instantiate and return AccountRepository
                return new AccountRepository(cosmosDBConnection, databaseName, containerName);
            });

            // Register RabbitMQConnection as a Singleton
            builder.Services.AddSingleton<RabbitMQConnection>(sp =>
            {
                var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER");
                var rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
                
                if (string.IsNullOrEmpty(rabbitMqHost) || string.IsNullOrEmpty(rabbitMqUser) || string.IsNullOrEmpty(rabbitMqPassword))
                {
                    throw new InvalidOperationException("RabbitMQ connection details are not set.");
                }
                return new RabbitMQConnection(rabbitMqHost, rabbitMqUser, rabbitMqPassword);
            });

            // Register CosmosDBConnection as a Singleton
            builder.Services.AddSingleton<CosmosDBConnection>(sp =>
            {
                var connectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Cosmos DB connection string is not set.");
                }
                return new CosmosDBConnection(connectionString);
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

            app.Urls.Add("http://0.0.0.0:8084");
            //app.Urls.Add("https://0.0.0.0:8085");
            app.MapControllers();

            app.Run();
        }
    }
}
