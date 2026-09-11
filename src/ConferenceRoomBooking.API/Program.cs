using ConferenceRoomBooking.Application.Interfaces;
using ConferenceRoomBooking.Application.Behaviors;
using ConferenceRoomBooking.Application.Features;
using ConferenceRoomBooking.Application.Services;
using ConferenceRoomBooking.API.Middleware;
using ConferenceRoomBooking.Domain.Interfaces;
using ConferenceRoomBooking.Infrastructure.Persistence;
using ConferenceRoomBooking.Infrastructure.Repositories;
using ConferenceRoomBooking.Infrastructure.Reports;
using ConferenceRoomBooking.Application.Services.Reports;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateHallCommandValidator).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<CreateHallCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

string jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
string jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ConferenceRoomBooking.Api";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtIssuer,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });
builder.Services.AddAuthorization();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=conference_booking.db";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddScoped<IHallRepository, HallRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IServiceCatalog, ServiceCatalog>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IReportBuilder, BusinessReportBuilder>();
builder.Services.AddScoped<ReportDirector>();
builder.Services.AddScoped<IPricingStrategy, TimeSlotPricingStrategy>();
builder.Services.AddScoped<PricingContext>();
builder.Services.AddScoped<IPricingService, PricingService>();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DatabaseInitializer databaseInitializer = new DatabaseInitializer(context);
    databaseInitializer.InitializeAsync().GetAwaiter().GetResult();

    DatabaseSeeder databaseSeeder = new DatabaseSeeder(context);
    databaseSeeder.SeedAsync().GetAwaiter().GetResult();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program
{
}
