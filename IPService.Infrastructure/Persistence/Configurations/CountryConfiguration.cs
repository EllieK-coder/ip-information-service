using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IPService.Domain.Entities;

namespace IPService.Infrastructure.Persistence.Configurations
{

    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> entity)
        {
            entity.ToTable("Countries");
            entity.HasKey(c => c.TwoLetterCode);
            entity.Property(c => c.TwoLetterCode)
                .HasColumnType("char(2)")
                .IsRequired();
            entity.Property(c => c.ThreeLetterCode)
                .HasColumnType("char(3)")
                .IsRequired();
            entity.HasIndex(c => c.ThreeLetterCode)
                .IsUnique();
            entity.Property(c => c.Name)
                .HasColumnName("CountryName")
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}