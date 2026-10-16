using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
namespace GymManagmentSystem.DAL.Repositories.Classes;
public class UnitOfWork : IUnitOfWork
{
    private readonly GymDbContext _dbContext;
    public IMemberRepository memberRepository { get; set; }
    public UnitOfWork(GymDbContext dbContext, IMemberRepository memberRepository)
    {
        _dbContext = dbContext;
        this.memberRepository = memberRepository;
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
