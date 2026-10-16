using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmentSystem.DAL.Data.Configurations;

public class MemberSessionConfigurations : IEntityTypeConfiguration<MemberSession>
{
    public void Configure(EntityTypeBuilder<MemberSession> builder)
    {
        builder.Ignore(x => x.Id);

        builder.Property(x => x.CreatedAt)
               .HasColumnName("BookingDate")
               .HasDefaultValueSql("GETDATE()");

        builder.HasKey(x => new { x.MemberId, x.SessionId });
    }
}
