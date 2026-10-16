using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GymManagmentSystem.DAL.Data.Configurations;
public class HealthRecoedConfigurations : IEntityTypeConfiguration<HealthRecoed>
{
    public void Configure(EntityTypeBuilder<HealthRecoed> builder)
    {
        builder.Property(x => x.Height).HasPrecision(5, 2);
        builder.Property(x => x.Weight).HasPrecision(5, 2);
        builder.Property(x => x.BloodType).HasMaxLength(3);
    }
}
