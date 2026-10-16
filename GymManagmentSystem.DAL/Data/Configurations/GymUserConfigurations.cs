using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmentSystem.DAL.Data.Configurations;

public class GymUserConfigurations<T> : IEntityTypeConfiguration<T> where T : GymUser, new()
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.Name)
               .HasColumnType("varchar")
               .HasMaxLength(50);

        builder.Property(x => x.Email)
               .HasColumnType("varchar")
               .HasMaxLength(100);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Phone).IsUnique();

        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("EmailCheck", "Email like '_%@_%._%'");
            tb.HasCheckConstraint("PhoneCheck",
                "LEN(Phone) = 11 AND Phone NOT LIKE '%[^0-9]%' AND (Phone like '010%' or Phone like '011%' or Phone like '012%' or Phone like '015%')");
        });

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(x => x.Street).HasColumnName("Street").HasColumnType("varchar").HasMaxLength(30);
            address.Property(x => x.City).HasColumnName("City").HasColumnType("varchar").HasMaxLength(30);
            address.Property(x => x.BuildingNumber).HasColumnName("BuildingNumber");
        });
    }
}
