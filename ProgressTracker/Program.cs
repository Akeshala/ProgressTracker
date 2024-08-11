using Microsoft.EntityFrameworkCore;
using ProgressTracker.Data;
using ProgressTracker.Middleware;
using ProgressTracker.Services;
using ProgressTracker.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options => { options.Filters.Add<AuthorizeTokenAttribute>(); });
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IDailyRecordService, DailyRecordService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IWeeklyReportService, WeeklyReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IResultPredictorService, ResultPredictorService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient("ProgressTrackerUserService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5058/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient("ProgressTrackerSubjectService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5106/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthTokenHandler>();

// Configure Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseMiddleware<UserIdMiddleware>(); // Add middleware to the pipeline

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