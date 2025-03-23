
using ContentService.Helpers;
using ContentService.Interfaces;
using ContentService.Managers;
using DotNetEnv;

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


            // Register RabbitMQConnection as a Singleton
            builder.Services.AddSingleton<RabbitMQConnection>(sp =>
            {
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
