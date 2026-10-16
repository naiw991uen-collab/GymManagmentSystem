using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagmentSystem.DAL.Repositories.Classes;

public class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(GymDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<IEnumerable<Plan>> GetAllWithMemberShipAsync(bool tracking = false)
    {
        return tracking ? await _dbContext.Plans.Include(x=>x.MemberShips).ToListAsync() : await _dbContext.Plans.Include(x => x.MemberShips).AsNoTracking().ToListAsync();
    }
}
