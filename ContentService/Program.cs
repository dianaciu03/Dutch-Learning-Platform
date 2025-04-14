
using ContentService.Helpers;
using ContentService.Interfaces;
using ContentService.Managers;
using DotNetEnv;
using Prometheus;

namespace ContentService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLogging();


            // Check if the environment is Docker (from GitHub CI/CD pipeline)
            var dockerEnv = Environment.GetEnvironmentVariable("DOCKER_ENV");
            var integrationTestEnv = Environment.GetEnvironmentVariable("INTEGRATION_TEST_ENV");
            Console.WriteLine($"DOCKER_ENV: {dockerEnv}");

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

            // Register RabbitMQConnection as a Singleton
            builder.Services.AddSingleton<RabbitMQConnection>(sp =>
            {
                string rabbitMqHost = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_HOST");
                string rabbitMqUser = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_USER");
                string rabbitMqPassword = Environment.GetEnvironmentVariable($"{envPrefix}RABBITMQ_PASSWORD");

                if (string.IsNullOrEmpty(rabbitMqHost) || string.IsNullOrEmpty(rabbitMqUser) || string.IsNullOrEmpty(rabbitMqPassword))
                {
                    throw new InvalidOperationException("RabbitMQ connection details are not set.");
                }
                
                // Return the RabbitMQ connection using the values fetched from the environment variables
                return new RabbitMQConnection(rabbitMqHost, rabbitMqUser, rabbitMqPassword);
            });

            builder.Services.AddHostedService<RabbitMQListener>();

            // Add services to the container
            builder.Services.AddScoped(typeof(LogHelper<>));
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
                                  // Add the /metrics endpoint for Prometheus

            app.MapMetrics();  // This exposes metrics at the /metrics endpoint

            //app.UseHttpsRedirection();
            app.UseAuthorization();
            
            if (dockerEnv == "true")
            {
                // DOCKER ONLY
                app.Urls.Add("http://0.0.0.0:8082");
                //app.Urls.Add("https://0.0.0.0:8083");
            }

            app.MapControllers();

            app.Run();
        }
    }
}
