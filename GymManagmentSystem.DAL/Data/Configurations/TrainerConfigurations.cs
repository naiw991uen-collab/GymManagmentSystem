using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmentSystem.DAL.Data.Configurations;
public class TrainerConfigurations : GymUserConfigurations<Trainer>
{
    public new void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.Property(x => x.CreatedAt)
               .HasColumnName("HiringDate")
               .HasDefaultValue("GETDATE()");

        base.Configure(builder);
    }
}
