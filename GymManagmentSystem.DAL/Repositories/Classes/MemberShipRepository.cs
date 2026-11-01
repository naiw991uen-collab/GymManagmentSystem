using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymManagmentSystem.DAL.Repositories.Classes;

public class MemberShipRepository : Repository<MemberShip>, IMemberShipRepository
{
    public MemberShipRepository(GymDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<MemberShip>> GetAllMemberShipsWithMebersAndPlansAsync(Expression<Func<MemberShip, bool>>? expression, CancellationToken cancellationToken = default)
    {
        IQueryable<MemberShip> query = _dbContext.MemberShips.AsNoTracking().Include(m => m.Plan).Include(m => m.Member);

        if(expression != null)
        {
            query = query.Where(expression);
        }

        return query.ToListAsync(cancellationToken);
    }
}
