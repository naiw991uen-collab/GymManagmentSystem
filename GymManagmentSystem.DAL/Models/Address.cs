using Microsoft.EntityFrameworkCore;

namespace GymManagmentSystem.DAL.Models;

[Owned]
public class Address
{

    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public int BuildingNumber { get; set; }
}
