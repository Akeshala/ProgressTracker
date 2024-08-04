using ProgressTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ISubjectService, SubjectService>();
builder.Services.AddSingleton<IDailyRecordService, DailyRecordService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IWeeklyReportService, WeeklyReportService>();

// Retrieve the token from configuration or environment variable
string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJha2VzaGFsYUBnbWFpbC5jb20iLCJqdGkiOiJmNzVhNzg1Zi1kNDQ4LTRmOWYtOTI5Yy1hYjAxNjIwYTkyMjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJleHAiOjE3MjI3NTc0NTIsImlzcyI6InlvdXJkb21haW4uY29tIiwiYXVkIjoieW91cmRvbWFpbi5jb20ifQ.VLkxM_M5E69VXmuQlog6GCHU-efe2jJ0CYnMXxlH_WI";

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