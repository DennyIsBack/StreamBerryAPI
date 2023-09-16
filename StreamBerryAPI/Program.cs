using Microsoft.EntityFrameworkCore;
using StreamBerryAPI.Data;
using StreamBerryAPI.Repository;
using StreamBerryAPI.Repository.Interface;

namespace StreamBerryAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddEntityFrameworkSqlServer()
                            .AddDbContext<FilmDBContext>(
                            options => options.UseSqlite(builder.Configuration.GetConnectionString("DataBase"))
                );

            builder.Services.AddScoped<IFilmRepository, FilmRepository>();

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