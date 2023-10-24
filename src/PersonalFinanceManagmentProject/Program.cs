
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Services;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject
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
            builder.Services.AddTransient<IBillService, BillService>();
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddDbContext<PersonalFinanceManagmentDbContext>(
                    option => option.UseSqlServer(builder.Configuration.GetConnectionString("PersonalFinanceManagmentConnectionString"))
                );

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

public partial class Program { }