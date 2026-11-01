using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
namespace GymManagmentSystem.DAL.Repositories.Classes;
public class UnitOfWork : IUnitOfWork
{
    private readonly GymDbContext _dbContext;
    public IMemberRepository memberRepository { get; }
    public ITrainerRepository TrainerRepository { get; }

    public ISessionRepository SessionRepository { get; }
    public IMemberShipRepository MemberShipRepository { get; }

    public IBookingRepository BookingRepository { get; }

    public UnitOfWork(GymDbContext dbContext, IMemberRepository memberRepository, ITrainerRepository trainerRepository, ISessionRepository sessionRepository, IMemberShipRepository memberShipRepository, IBookingRepository bookingRepository)
    {
        _dbContext = dbContext;
        this.memberRepository = memberRepository;
        this.TrainerRepository = trainerRepository;
        SessionRepository = sessionRepository;
        MemberShipRepository = memberShipRepository;
        BookingRepository = bookingRepository;
    }
    private Dictionary<string,object> Repos = [];

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
    {
        var type = typeof(TEntity).Name;

        if (!string.IsNullOrEmpty(type) && Repos.TryGetValue(type,out object value))
        {
            return (IRepository<TEntity>)Repos[type];
        }
        var repo = new Repository<TEntity>(_dbContext);

        Repos[type] = repo;

        return repo;
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();

    }

}
