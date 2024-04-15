using AWC.PersonData.API.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AWC.PersonData.API.Infrastructure.Persistence.Configurations;

internal class EmailAddressConfig : IEntityTypeConfiguration<EmailAddress>
{
    public void Configure(EntityTypeBuilder<EmailAddress> entity)
    {
        entity.ToTable("EmailAddress", schema: "Person");
        entity.HasKey(e => e.EmailAddressID);

        entity.Property(e => e.BusinessEntityID)
            .HasColumnName("BusinessEntityID")
            .ValueGeneratedNever();
        entity.Property(e => e.EmailAddressID)
            .HasColumnName("EmailAddressID")
            .ValueGeneratedOnAdd();
        entity.Property(e => e.MailAddress)
            .IsRequired()
            .HasColumnName("EmailAddress")
            .HasColumnType("nvarchar(50)");
        entity.Property(e => e.RowGuid)
            .HasColumnName("rowguid")
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired()
            .HasDefaultValue(Guid.NewGuid());
        entity.Property(e => e.ModifiedDate)
            .HasColumnName("ModifiedDate")
            .IsRequired()
            .HasDefaultValue(DateTime.Now);
    }
}
