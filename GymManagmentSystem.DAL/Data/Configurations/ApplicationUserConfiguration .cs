using GymManagmentSystem.DAL.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmentSystem.DAL.Data.Configurations;
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName)
            .HasColumnType("varchar")
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .HasColumnType("varchar")
            .HasMaxLength(50);
    }
}
