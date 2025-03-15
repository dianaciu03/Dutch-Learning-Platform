using DotNetEnv;
using UserService.Helpers;
using UserService.Interfaces;
using UserService.Managers;

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

            // Register RabbitMQConnection as a Singleton (or Scoped if you prefer)
            builder.Services.AddSingleton<RabbitMQConnection>(sp =>
            {
                var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER");
                var rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

                return new RabbitMQConnection(rabbitMqHost, rabbitMqUser, rabbitMqPassword);
            });

            // Add services to the container.
            builder.Services.AddScoped(typeof(LogHelper<>));
            builder.Services.AddScoped<IAccountManager, AccountManager>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
