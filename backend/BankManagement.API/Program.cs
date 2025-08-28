using Microsoft.EntityFrameworkCore;
using BankManagement.API.Data;
using BankManagement.API.Interfaces;
using BankManagement.API.Repositories;
using BankManagement.API.Services;
using BankManagement.API.Configurations;
using FluentValidation.AspNetCore;
using FluentValidation;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/bank-management-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// FluentValidation configuration
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<Program>();

// Database configuration
builder.Services.AddDbContext<BankDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Repository registration
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Service registration
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Bank Management System API",
        Version = "v1",
        Description = "نظام إدارة البنك - واجهة برمجة التطبيقات",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "فريق التطوير",
            Email = "dev@bankmanagement.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS configuration
var allowedOrigins = builder.Configuration["CORS:AllowedOrigins"]?.Split(';') ?? 
    new[] { "http://localhost:3000", "https://localhost:3000" };
    
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

// Configure Kestrel for production
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Use CORS before other middleware
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

// Global error handling
app.UseMiddleware<ErrorHandlingMiddleware>();

// Custom middleware for request/response logging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Request: {Method} {Path} from {RemoteIpAddress}",
        context.Request.Method,
        context.Request.Path,
        context.Connection.RemoteIpAddress);

    await next();

    logger.LogInformation("Response: {StatusCode} for {Method} {Path}",
        context.Response.StatusCode,
        context.Request.Method,
        context.Request.Path);
});

// Global exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exceptionFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        
        if (exceptionFeature != null)
        {
            logger.LogError(exceptionFeature.Error, "Unhandled exception occurred");
            
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                success = false,
                message = "حدث خطأ داخلي في الخادم",
                timestamp = DateTime.UtcNow,
                traceId = context.TraceIdentifier
            };
            
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    });
});

// Root endpoint with API information
app.MapGet("/", () => Results.Json(new 
{
    status = "Bank Management System API is running ✅",
    version = "v1",
    documentation = "/swagger",
    healthCheck = "/health",
    timestamp = DateTime.UtcNow
}));

// Serve static files (for frontend)
app.UseStaticFiles();

// Fallback for SPA routing - only in production
if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

// Database initialization
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<BankDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Ensuring database is created...");
        await context.Database.EnsureCreatedAsync();
        
        // Apply pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Applying pending migrations...");
            await context.Database.MigrateAsync();
        }
        
        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

// Application startup logging
Log.Information("Bank Management System API started successfully");
Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
Log.Information("Swagger UI available at: /swagger");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
