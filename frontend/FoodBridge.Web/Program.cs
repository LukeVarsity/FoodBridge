using FoodBridge.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

/* Typed HttpClient with IHttpClientFactory
 * Microsoft Learn
 * Accessed : 9 October 2026
 * Available at : https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests
 */
// ApiBaseUrl is localhost:5001 for dotnet run, the foodbridge-api alias in docker compose,
// and the api's onrender.com url on render
builder.Services.AddHttpClient<ListingApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// no https redirect here, render handles https in front of the container
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
