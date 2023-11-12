
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Exceptions;
using PersonalFinanceManagmentProject.Filters;
using PersonalFinanceManagmentProject.Services;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddDbContext<PersonalFinanceManagmentDbContext>(
                option => option.UseSqlServer(builder.Configuration.GetConnectionString("PersonalFinanceManagmentConnectionString"))
            );

            // Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            
            // Filters
            builder.Services.AddControllers();
            builder.Services.AddControllers(o =>
            {
                o.Filters.Add<EntityNotFoundExceptionFilter>();
                o.Filters.Add<BadStatusExceptionFilter>();
                o.Filters.Add<ParameterNullExceptionFilter>();
            });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<IBillService, BillService>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
           
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

public partial class Program { }