// PROMPT v2.2: Інтеграційний тест JWT і API endpoint-ів

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ConferenceRoomBooking.Tests;

public class ApiIntegrationTests
{
  [Fact]
  public async Task ProtectedHallCreation_ShouldRequireJwtAndAcceptAdminToken()
  {
    string databasePath = Path.Combine(Path.GetTempPath(), $"conference-booking-{Guid.NewGuid():N}.db");
    WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(builder =>
      {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={databasePath}");
      });

    try
    {
      HttpClient client = factory.CreateClient();
      HttpResponseMessage unauthorizedResponse = await client.PostAsJsonAsync("/api/halls", new
      {
        name = "Unauthorized Hall",
        capacity = 20,
        hourlyRate = 1000,
        serviceIds = Array.Empty<Guid>(),
      });
      Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);

      HttpResponseMessage tokenResponse = await client.PostAsJsonAsync("/api/auth/token", new
      {
        username = "admin",
        password = "ChangeThisDevelopmentPassword!",
      });
      Assert.Equal(HttpStatusCode.OK, tokenResponse.StatusCode);
      JsonDocument tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
      string token = tokenJson.RootElement.GetProperty("accessToken").GetString() ?? string.Empty;
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      HttpResponseMessage authorizedResponse = await client.PostAsJsonAsync("/api/halls", new
      {
        name = "Integration Hall",
        capacity = 20,
        hourlyRate = 1000,
        serviceIds = Array.Empty<Guid>(),
      });

      Assert.Equal(HttpStatusCode.Created, authorizedResponse.StatusCode);
    }
    finally
    {
      factory.Dispose();
    }
  }
}