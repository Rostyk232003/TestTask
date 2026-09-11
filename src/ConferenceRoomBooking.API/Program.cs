using ConferenceRoomBooking.Application.Interfaces;
using ConferenceRoomBooking.Application.Behaviors;
using ConferenceRoomBooking.Application.Features;
using ConferenceRoomBooking.Application.Services;
using ConferenceRoomBooking.API.Middleware;
using ConferenceRoomBooking.Domain.Interfaces;
using ConferenceRoomBooking.Infrastructure.Persistence;
using ConferenceRoomBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateHallCommandValidator).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<CreateHallCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

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
builder.Services.AddScoped<IPricingStrategy, TimeSlotPricingStrategy>();
builder.Services.AddScoped<PricingContext>();
builder.Services.AddScoped<IPricingService, PricingService>();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

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
app.UseAuthorization();
app.MapControllers();

app.Run();
