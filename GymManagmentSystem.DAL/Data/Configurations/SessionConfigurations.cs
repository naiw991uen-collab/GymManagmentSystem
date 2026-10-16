using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmentSystem.DAL.Data.Configurations
{
    public class SessionConfigurations : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CapacityCheck", "Capacity Between 1 and 25");
                tb.HasCheckConstraint("EndDateCheck",
                    "EndDate > StartDate");
            });
        }
    }
}
