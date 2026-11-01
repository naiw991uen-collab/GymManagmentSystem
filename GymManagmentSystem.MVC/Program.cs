using FluentValidation;
using FluentValidation.AspNetCore;
using GymManagmentSystem.BLL.Extentions.DependencyEnjection;
using GymManagmentSystem.BLL.Services.Members.Validators;
using GymManagmentSystem.DAL.Applications;
using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Extentions.DependencyEnjection;
using GymManagmentSystem.PL.Extentions;
using GymManagmentSystem.PL.Extentions.Logging;
using GymManagmentSystem.PL.Hubs.Class;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.Identity;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

// Logging
builder.AddSerilogLogging();

// MVC
builder.Services.AddControllersWithViews();

// Serial R
builder.Services.AddSignalR();

// Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberViewModelValidator>();

// Layers
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();
builder.Services.AddScoped<IDashboardNotifier, DashboardNotifier>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
{
    config.User.RequireUniqueEmail = true;
    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    config.Lockout.MaxFailedAccessAttempts = 5;
}).AddEntityFrameworkStores<GymDbContext>();

var app = builder.Build();

await app.SeedDataAndApplyPendingMigrations();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); 
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapHub<DashboardHub>("/hubs/dashboard");

app.Run();
