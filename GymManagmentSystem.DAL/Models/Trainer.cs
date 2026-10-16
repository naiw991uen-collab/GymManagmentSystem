using GymManagmentSystem.DAL.Models.Enums;

namespace GymManagmentSystem.DAL.Models;
public class Trainer : GymUser
{
    // HireDate => CreatedAt
    public Specialty Specialty  { get; set; }
    public ICollection<Session> Sessions { get; set; } = default!;
    public int TrainerId { get; set; }
    

}
