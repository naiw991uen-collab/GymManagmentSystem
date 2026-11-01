using GymManagmentSystem.DAL.Models;

namespace GymManagmentSystem.DAL.Repositories.Interfaces;
public interface ITrainerRepository : IRepository<Trainer>
{
    public IQueryable<Trainer> GetTrainerWithRelatedData(int id);
}
