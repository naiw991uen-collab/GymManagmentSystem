using GymManagmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmentSystem.DAL.Data.Configurations;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.CategoryName)
             .HasColumnType("varchar")
             .HasMaxLength(20);

        builder.Property(x => x.CreatedAt)
              .HasColumnName("HiringDate");

        builder.HasData(
            new Category() { Id = 1, CategoryName = "Strength" },
            new Category() { Id = 2, CategoryName = "Cardio" },
            new Category() { Id = 3, CategoryName = "Yoga" },
            new Category() { Id = 4, CategoryName = "Boxing" },
            new Category() { Id = 5, CategoryName = "CrosFit" });
    }
}
