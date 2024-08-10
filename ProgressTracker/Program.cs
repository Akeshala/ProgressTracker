using Microsoft.EntityFrameworkCore;
using ProgressTracker.Data;
using ProgressTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IDailyRecordService, DailyRecordService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IWeeklyReportService, WeeklyReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IResultPredictorService, ResultPredictorService>();

// Retrieve the token from configuration or environment variable
string token =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwianRpIjoiMzRmMTBkY2QtOGI0YS00ODBjLWIxNTgtZjA5NjgxNTQ5NTdhIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxIiwiZXhwIjoxNzIzMzEyNzU3LCJpc3MiOiJ5b3VyZG9tYWluLmNvbSIsImF1ZCI6InlvdXJkb21haW4uY29tIn0.vCvCSMItFKbeLETXN63ltDrJ4kNRvlcM7Cz5cJuz74k";

builder.Services.AddHttpClient("ProgressTrackerUserService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5058/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
});

builder.Services.AddHttpClient("ProgressTrackerSubjectService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5106/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
});

// Configure Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();