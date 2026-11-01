using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagmentSystem.DAL.Repositories.Classes;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(GymDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory()
    {
        return await _dbContext.Sessions.AsNoTracking().Include(x => x.Trainer).Include(x => x.Category).ToListAsync();
    }

    public async Task<int> GetCountOfBookedSlotsAsnc(int sessionId)
    {
        return await _dbContext.memberSessions.AsNoTracking().Where(x=>x.SessionId == sessionId).CountAsync();
    }

    public async Task<Session?> GetSessionWithTrainerAndCategory(int id)
    {
        return await _dbContext.Sessions
            .Include(s => s.Trainer)
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> IsTrainerBusyAsync(int trainerId, DateTime start, DateTime end, int excludeSessionId)
    {
        return await _dbContext.Sessions.AnyAsync(s =>
            s.TrainerId == trainerId &&
            s.Id != excludeSessionId &&
            s.StartDate < end &&
            s.EndDate > start);
    }
}
