using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagmentSystem.DAL.Repositories.Classes
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, new()
    {
        protected readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _set;
        public Repository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = _dbContext.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
           await _set.AddAsync(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _set.Remove(entity);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false)
        {
            return tracking ? await _set.ToListAsync() : await _set.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _set.FindAsync(id);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _set.Update(entity);
        }
    }
}
