using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IPService.Domain.Entities;

namespace IPService.Infrastructure.Persistence.Configurations
{

    public class Ipv4AddressConfiguration : IEntityTypeConfiguration<Ipv4Address>
    {
        public void Configure(EntityTypeBuilder<Ipv4Address> entity)
        {

            entity.ToTable("Ipv4Address");
            entity.HasKey(i => i.AddressId);
            entity.Property(i => i.AddressId)
                .ValueGeneratedNever()
                .IsRequired();
            entity.Property(i => i.Address)
                .HasColumnType("varchar(15)")
                .IsRequired();
            entity.HasIndex(i => i.Address)
                .IsUnique();
            entity.Property(i => i.TwoLetterCode)
                .HasColumnType("char(2)")
                .IsRequired();
            entity.Property(i => i.LastUpdated)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime2")
                .IsRequired();
            entity.HasOne<Country>()
                .WithMany()
                .HasForeignKey(i => i.TwoLetterCode)
                .OnDelete(DeleteBehavior.Restrict);//if delete country , kepp ips 

            // Covering Index για performance (Task 3: Get Report)
            // GROUP BY CountryTwoLetterCode με MAX(LastUpdated)
            // Με INCLUDE, query τρέχει μόνο στο index
            entity.HasIndex(i => i.TwoLetterCode)
                .IncludeProperties(i => i.LastUpdated)
                .HasDatabaseName("IX_Ipv4Address_TwoLetterCode_LastUpdated");
        }
    }
}