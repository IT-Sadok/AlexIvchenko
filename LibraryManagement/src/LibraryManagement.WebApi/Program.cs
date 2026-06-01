using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Validators;
using LibraryManagement.Infrastructure.DependencyInjection;
using LibraryManagement.WebApi.Endpoints;

const string ReactCorsPolicy = "ReactCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(ReactCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

/* removed controllers */
//builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookValidator, BookValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors(ReactCorsPolicy);

/* removed controllers */
//app.MapControllers();

app.MapBookEndpoints();

app.Run();
