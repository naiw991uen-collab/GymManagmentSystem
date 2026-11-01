using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Classes;

namespace GymManagmentSystem.DAL.Repositories.Interfaces;
public interface IUnitOfWork
{
    public IMemberRepository memberRepository { get; }
    public ITrainerRepository TrainerRepository { get; }
    public ISessionRepository SessionRepository { get; }
    public IMemberShipRepository MemberShipRepository { get; }
    public IBookingRepository BookingRepository { get; }
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
    Task<int> SaveChangesAsync();
}
