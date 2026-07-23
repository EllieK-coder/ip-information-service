using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IPService.Domain.Entities;

namespace IPService.Infrastructure.Persistence.Configurations
{
    public class JobHistoryConfiguration : IEntityTypeConfiguration<JobHistory>
    {
        public void Configure(EntityTypeBuilder<JobHistory> entity)
        {
            entity.ToTable("JobHistory");

            entity.HasKey(j => j.Id);
            entity.Property(j => j.Id)
                .ValueGeneratedOnAdd();//auto increment

            entity.Property(j => j.StartedAt)
                .IsRequired();

            entity.Property(j => j.FinishedAt)
                .IsRequired();

            entity.Property(j => j.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(j => j.ProcessedRecords)
                .IsRequired();

            entity.Property(j => j.UpdatedRecords)
                .IsRequired();

            entity.Property(j => j.FailedRecords)
                .IsRequired();
        }
    }
}