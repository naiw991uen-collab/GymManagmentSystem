using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;

namespace GymManagmentSystem.DAL.Repositories.Classes;
public interface IMemberRepository : IRepository<Member>
{
    public IQueryable<Member> GetMemberWithRelatedData(int id);
}
