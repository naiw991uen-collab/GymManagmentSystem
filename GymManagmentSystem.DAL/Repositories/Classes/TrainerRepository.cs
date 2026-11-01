using GymManagmentSystem.DAL.Contexts;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagmentSystem.DAL.Repositories.Classes;

public class TrainerRepository : Repository<Trainer>, ITrainerRepository
{
    public TrainerRepository(GymDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<Trainer> GetTrainerWithRelatedData(int id)
    {
        return _dbContext.Trainers.Where(x => x.Id == id).Include(x => x.Sessions);
    }
}
