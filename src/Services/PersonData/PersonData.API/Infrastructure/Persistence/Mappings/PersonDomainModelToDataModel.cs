using Mapster;
using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Infrastructure.Persistence.DataModels;

namespace AWC.PersonData.API.Infrastructure.Persistence.Mappings;

public sealed class PersonDomainModelToDataModel : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // TypeAdapterConfig<TSource, TDestination> ((DateTime?)null)

        _ = config.NewConfig<Person, PersonDataModel>()
        .Map(dest => dest.BusinessEntityID, src => src.Id.Value)
        .Map(dest => dest.PersonType, src => src.PersonType.Value)
        .Map(dest => dest.NameStyle, src => src.NameStyle != NameStyle.Western)
        .Map(dest => dest.Title, src => src.Title.Value)
        .Map(dest => dest.FirstName, src => src.Name.FirstName)
        .Map(dest => dest.MiddleName, src => src.Name.MiddleName)
        .Map(dest => dest.LastName, src => src.Name.LastName)
        .Map(dest => dest.Suffix, src => src.Suffix.Value)
        .Map(dest => dest.EmailPromotion, src => (int)src.EmailPromotions)
        .Ignore(dest => dest.EmailAddresses)
        .Ignore(dest => dest.BusinessEntityAddresses)
        .Ignore(dest => dest.Telephones);

        _ = config.NewConfig<AWC.PersonData.API.Domain.PersonAggregate.Address, AWC.PersonData.API.Infrastructure.Persistence.DataModels.Address>()
        .Map(dest => dest.AddressID, src => src.Id.Value)
        .Map(dest => dest.AddressLine1, src => src.Location.AddressLine1)
        .Map(dest => dest.AddressLine2, src => src.Location.AddressLine2)
        .Map(dest => dest.City, src => src.Location.City)
        .Map(dest => dest.StateProvinceID, src => src.Location.StateProvinceID)
        .Map(dest => dest.PostalCode, src => src.Location.PostalCode);

        _ = config.NewConfig<AWC.PersonData.API.Domain.PersonAggregate.PersonPhone, AWC.PersonData.API.Infrastructure.Persistence.DataModels.PersonPhone>()
        .Map(dest => dest.PhoneNumber, src => src.Telephone.Value)
        .Map(dest => dest.PhoneNumberTypeID, src => (int)src.PhoneNumberType);

        _ = config.NewConfig<AWC.PersonData.API.Domain.PersonAggregate.PersonEmailAddress, AWC.PersonData.API.Infrastructure.Persistence.DataModels.EmailAddress>()
        .Map(dest => dest.EmailAddressID, src => src.EmailAddressID.Value)
        .Map(dest => dest.MailAddress, src => src.EmailAddress.Value);
    }
}
