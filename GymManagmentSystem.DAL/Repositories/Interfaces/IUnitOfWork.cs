using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Classes;

namespace GymManagmentSystem.DAL.Repositories.Interfaces;
public interface IUnitOfWork
{
    public IMemberRepository memberRepository { get; set; }
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
    Task<int> SaveChangesAsync();
}
