
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

            var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
            Env.Load(envFilePath);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();

            // Register RabbitMQConnection as a Singleton
            builder.Services.AddSingleton<RabbitMQConnection>(sp =>
            {
                var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER");
                var rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

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
            app.Urls.Add("http://0.0.0.0:8082");
            //app.Urls.Add("https://0.0.0.0:8083");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
