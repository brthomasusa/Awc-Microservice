using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AWC.PersonData.API.Infrastructure.Persistence.Configurations;

internal class PersonPhoneConfig : IEntityTypeConfiguration<PersonPhone>
{
    public void Configure(EntityTypeBuilder<PersonPhone> entity)
    {
        entity.ToTable("PersonPhone", schema: "Person");
        entity.HasKey(e => new { e.BusinessEntityID, e.PhoneNumber, e.PhoneNumberTypeID });
        entity.HasIndex(p => p.PhoneNumber).IsUnique();

        entity.Property(e => e.BusinessEntityID)
            .HasColumnName("BusinessEntityID")
            .ValueGeneratedNever();
        entity.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasColumnName("PhoneNumber")
            .HasColumnType("nvarchar(25)");
        entity.Property(e => e.PhoneNumberTypeID)
            .HasColumnName("PhoneNumberTypeID");
        entity.Property(e => e.ModifiedDate)
            .HasColumnName("ModifiedDate")
            .IsRequired()
            .HasDefaultValue(DateTime.Now);
    }
}
