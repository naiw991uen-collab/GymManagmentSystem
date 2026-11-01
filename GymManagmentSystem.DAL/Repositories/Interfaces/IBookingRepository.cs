using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.DAL.Repositories.Interfaces;
public interface IBookingRepository : IRepository<MemberSession>
{
    public Task<IEnumerable<MemberSession>> GetSessionByIdAsync(int sessionid , CancellationToken ct = default);
}
