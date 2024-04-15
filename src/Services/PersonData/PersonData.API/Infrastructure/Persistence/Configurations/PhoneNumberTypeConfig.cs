using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AWC.PersonData.API.Infrastructure.Persistence.Configurations;

internal class PhoneNumberTypeConfig : IEntityTypeConfiguration<PhoneNumberType>
{
    public void Configure(EntityTypeBuilder<PhoneNumberType> entity)
    {
        entity.ToTable("PhoneNumberType", schema: "Person");
        entity.HasKey(e => e.PhoneNumberTypeID);
        entity.HasIndex(p => p.Name).IsUnique();

        entity.Property(e => e.PhoneNumberTypeID)
            .HasColumnName("PhoneNumberTypeID")
            .ValueGeneratedOnAdd();
        entity.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasColumnType("nvarchar(50)");
        entity.Property(e => e.ModifiedDate)
            .HasColumnName("ModifiedDate")
            .IsRequired()
            .HasDefaultValue(DateTime.Now);
    }
}
