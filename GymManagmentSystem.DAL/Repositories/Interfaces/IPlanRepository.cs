using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.DAL.Repositories.Interfaces;
public interface IPlanRepository : IRepository<Plan>
{
    public Task<IEnumerable<Plan>> GetAllWithMemberShipAsync(bool tracking = false);

}
