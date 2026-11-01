using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagmentSystem.DAL.Repositories.Classes;

public class BookingRepository : Repository<MemberSession>, IBookingRepository
{
    public BookingRepository(GymDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<MemberSession>> GetSessionByIdAsync(int sessionid, CancellationToken ct = default)
        => await _dbContext.memberSessions.AsNoTracking()
        .Include(m => m.Member).Where(m => m.SessionId == sessionid)
        .ToListAsync();
}
