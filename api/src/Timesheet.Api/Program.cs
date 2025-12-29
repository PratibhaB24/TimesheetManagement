using Timesheet.Application;
using Timesheet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ============ ADD SERVICES TO THE CONTAINER ============

// Add Application Layer services (AutoMapper, Business Services)
builder.Services.AddApplication();

// Add Infrastructure Layer services (DbContext, Repositories, UnitOfWork)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Timesheet Management API",
        Version = "v1",
        Description = "A RESTful API for managing employee timesheets, projects, and assignments."
    });
});

// Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ============ CONFIGURE THE HTTP REQUEST PIPELINE ============

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timesheet Management API v1");
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
