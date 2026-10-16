using FluentValidation;
using FluentValidation.AspNetCore;
using GymManagmentSystem.BLL.Services.Members.Classes;
using GymManagmentSystem.BLL.Services.Members.Interfaces;
using GymManagmentSystem.BLL.Services.Members.Validators;
using GymManagmentSystem.BLL.Services.Plansa.Classes;
using GymManagmentSystem.BLL.Services.Plansa.Interfaces;
using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Repositories.Classes;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberViewModelValidator>();


builder.Services.AddDbContext<GymDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Allow Dependency Enjiction
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); 
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
