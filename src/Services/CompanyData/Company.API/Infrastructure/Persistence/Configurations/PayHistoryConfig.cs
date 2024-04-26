using AWC.Company.API.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AWC.Company.API.Infrastructure.Persistence.Configurations
{
    internal class PayHistoryConfig : IEntityTypeConfiguration<EmployeePayHistory>
    {
        public void Configure(EntityTypeBuilder<EmployeePayHistory> entity)
        {
            entity.ToTable("EmployeePayHistory", schema: "HumanResources");
            entity.HasKey(e => new { e.BusinessEntityID, e.RateChangeDate });

            entity.Property(e => e.BusinessEntityID)
                .HasColumnName("BusinessEntityID")
                .ValueGeneratedNever();
            entity.Property(e => e.RateChangeDate)
                .HasColumnName("RateChangeDate");
            entity.Property(e => e.Rate)
                .IsRequired()
                .HasColumnName("Rate")
                .HasColumnType("money");
            entity.Property(e => e.PayFrequency)
                .IsRequired()
                .HasColumnType("tinyint")
                .HasColumnName("PayFrequency");
            entity.Property(e => e.ModifiedDate)
                .HasColumnName("ModifiedDate")
                .IsRequired()
                .HasDefaultValue(DateTime.Now);
        }
    }
}