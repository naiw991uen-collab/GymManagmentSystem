using GymManagmentSystem.DAL.Models;
namespace GymManagmentSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory();

        Task<int> GetCountOfBookedSlotsAsnc(int sessionId);

        Task<Session?> GetSessionWithTrainerAndCategory(int id);

        Task<bool> IsTrainerBusyAsync(int trainerId, DateTime start, DateTime end, int excludeSessionId);
    }
}
