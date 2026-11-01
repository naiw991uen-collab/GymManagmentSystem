using GymManagmentSystem.DAL.Models;
using System.Linq.Expressions;

namespace GymManagmentSystem.DAL.Repositories.Interfaces;
public interface IMemberShipRepository : IRepository<MemberShip>
{
    Task<List<MemberShip>> GetAllMemberShipsWithMebersAndPlansAsync(Expression<Func<MemberShip, bool>>? expression, CancellationToken cancellationToken = default);
}
