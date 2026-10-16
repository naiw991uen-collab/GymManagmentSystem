using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmentSystem.DAL.Data.Configurations;

public class MemberShipConfigurations : IEntityTypeConfiguration<MemberShip>
{
    public void Configure(EntityTypeBuilder<MemberShip> builder)
    {
        builder.Ignore(x => x.Id);
        builder.Ignore(x => x.Status);
        builder.Ignore(x => x.IsActive);

        builder.Property(x => x.CreatedAt)
                   .HasColumnName("StartDate")
                   .HasDefaultValueSql ("GETDATE()");

        builder.HasKey(x => new { x.MemberId, x.PlanId });
    }
}
