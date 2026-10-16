using GymManagmentSystem.DAL.Data.Configurations;
using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace GymManagmentSystem.DAL.Contexts;
public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<MemberShip> MemberShips { get; set; }
    public DbSet<MemberSession> memberSessions { get; set; }
}
