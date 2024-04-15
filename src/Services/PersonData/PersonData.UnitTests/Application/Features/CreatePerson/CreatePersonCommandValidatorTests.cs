using System.Text;
using AWC.PersonData.API.Application.Features.CreatePerson;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;
using AWC.PersonData.API.Domain.Interfaces;
using AWC.Shared.Kernel.Utilities;
using FluentValidation.TestHelper;
using Moq;
using PersonData.UnitTests.Data;

namespace PersonData.UnitTests.Application.Features.CreatePerson;

public class CreatePersonCommandValidatorTests
{
    private readonly Mock<IPersonRepository> _repoMock = new();
    private readonly CreatePersonCommandValidator? _validator;

    public CreatePersonCommandValidatorTests()
    {
        _repoMock.Setup(m => m.IsNameUniqueForCreate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });
        _repoMock.Setup(m => m.IsEmailUniqueForCreate(It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });
        _repoMock.Setup(m => m.IsValidStateProvinceId(It.IsAny<int>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });

        _validator = new(_repoMock.Object);
    }

    [Fact]
    public async void RuleFor_EmailAddress_CreatePersonCommandValidator_DuplicateEmail_ShouldReturn_Failure()
    {
        // Arrange
        Mock<IPersonRepository> mock = new();
        mock.Setup(m => m.IsNameUniqueForCreate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });
        mock.Setup(m => m.IsEmailUniqueForCreate(It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(false); });
        mock.Setup(m => m.IsValidStateProvinceId(It.IsAny<int>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });

        CreatePersonCommandValidator validator = new(mock.Object);
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommandWithDupeEmail();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public async void RuleFor_PersonName_CreatePersonCommandValidator_DuplicateName_ShouldReturn_Failure()
    {
        // Arrange
        Mock<IPersonRepository> mock = new();
        mock.Setup(m => m.IsNameUniqueForCreate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(false); });
        mock.Setup(m => m.IsEmailUniqueForCreate(It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });
        mock.Setup(m => m.IsValidStateProvinceId(It.IsAny<int>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });

        CreatePersonCommandValidator validator = new(mock.Object);
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async void RuleFor_StateProvinceId_CreatePersonCommandValidator_InvalidStateId_ShouldReturn_Failure()
    {
        // Arrange
        Mock<IPersonRepository> mock = new();
        mock.Setup(m => m.IsNameUniqueForCreate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });
        mock.Setup(m => m.IsEmailUniqueForCreate(It.IsAny<string>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(true); });
        mock.Setup(m => m.IsValidStateProvinceId(It.IsAny<int>()))
                .ReturnsAsync(() => { return Result<bool>.Success<bool>(false); });

        CreatePersonCommandValidator validator = new(mock.Object);
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public async void RuleFor_AllFields_CreatePersonCommandValidator_AllFieldsAreValid_ShouldReturn_Success()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("SC")]
    [InlineData("IN")]
    [InlineData("SP")]
    [InlineData("EM")]
    [InlineData("VC")]
    [InlineData("GC")]
    public async void RuleFor_PersonType_CreatePersonCommandValidator_ValidPersonTypes_ShouldReturn_Succeed(string personType)
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { PersonType = personType };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PersonType);
    }

    [Fact]
    public async void RuleFor_PersonType_CreatePersonCommandValidator_InvalidPersonType_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { PersonType = "XR" };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PersonType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async void RuleFor_NameStyle_CreatePersonCommandValidator_ValidData_ShouldReturn_Success(int nameStyle)
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { NameStyle = nameStyle };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NameStyle);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public async void RuleFor_NameStyle_CreatePersonCommandValidator_InvalidData_ShouldReturn_Failure(int nameStyle)
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { NameStyle = nameStyle };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NameStyle);
    }

    [Fact]
    public async void RuleFor_Title_CreatePersonCommandValidator_TooManyCharacters_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { Title = "abcdefghi" };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async void RuleFor_FirstName_CreatePersonCommandValidator_InvalidData_ShouldReturn_Failure(string? name)
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { FirstName = name! };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async void RuleFor_FirstName_CreatePersonCommandValidator_TooManyCharacters_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        StringBuilder sb = new("long-name") { Length = 61 };
        string name = sb.ToString();
        ;

        command = command with { FirstName = name };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async void RuleFor_LastName_CreatePersonCommandValidator_InvalidData_ShouldReturn_Failure(string? name)
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { LastName = name! };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async void RuleFor_LastName_CreatePersonCommandValidator_TooManyCharacters_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        StringBuilder sb = new("long-name") { Length = 61 };
        string name = sb.ToString();
        ;

        command = command with { LastName = name };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async void RuleFor_MiddleName_CreatePersonCommandValidator_TooManyCharacters_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        StringBuilder sb = new("long-name") { Length = 61 };
        string name = sb.ToString();
        ;

        command = command with { MiddleName = name };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MiddleName);
    }

    [Fact]
    public async void RuleFor_Suffix_CreatePersonCommandValidator_TooManyCharacters_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        StringBuilder sb = new("long-name") { Length = 11 };
        string name = sb.ToString();
        ;

        command = command with { Suffix = name };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Suffix);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async void RuleFor_EmailPromotion_CreatePersonCommandValidator_ValidData_ShouldReturn_Success(int emailPromo)
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { EmailPromotion = emailPromo };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EmailPromotion);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public async void RuleFor_EmailPromotion_CreatePersonCommandValidator_InvalidData_ShouldReturn_Failure(int emailPromo)
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { EmailPromotion = emailPromo };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailPromotion);
    }

    [Fact]
    public async void RuleFor_PersonPhone_CreatePersonCommandValidator_DuplicatePhoneNumberTypes_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<PersonPhoneDto> personPhoneDtos =
        [
            new (){PhoneNumber = "555-555-5555", PhoneNumberTypeID = 3},
            new (){PhoneNumber = "555-555-5555", PhoneNumberTypeID = 2},
            new (){PhoneNumber = "555-555-5555", PhoneNumberTypeID = 3}
        ];
        command = command with { Telephones = personPhoneDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Telephones);
    }

    [Fact]
    public async void RuleFor_PersonPhone_CreatePersonCommandValidator_EmptyList_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<PersonPhoneDto> personPhoneDtos = [];
        command = command with { Telephones = personPhoneDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Telephones);
    }

    [Fact]
    public async void RuleFor_EmailAddress_CreatePersonCommandValidator_DuplicateEmailAddresses_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<EmailAddressDto> emailAddressDtos =
        [
            new (){EmailAddressID = 1, MailAddress = "BOB@company.com"},
            new (){EmailAddressID = 2, MailAddress = "Bob@company.com"},
            new (){EmailAddressID = 3, MailAddress = "bob@company.com"}
        ];
        command = command with { EmailAddresses = emailAddressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailAddresses);
    }

    [Fact]
    public async void RuleFor_EmailAddress_CreatePersonCommandValidator_EmptyList_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<EmailAddressDto> emailAddressDtos = [];
        command = command with { EmailAddresses = emailAddressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailAddresses);
    }

    [Fact]
    public async void RuleFor_Addresses_CreatePersonCommandValidator_EmptyList_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<AddressDto> addressDtos = [];
        command = command with { Addresses = addressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Addresses);
    }

    [Fact]
    public async void RuleFor_Addresses_CreatePersonCommandValidator_DuplicateAddresses_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<AddressDto> addressDtos =
        [
            new (){ AddressID = 0, AddressLine1 = "1 Main St", AddressLine2 = "Apt 1", City = "Bigcity", StateProvinceID = 79, PostalCode = "98745", AddressTypeID =1 },
            new (){ AddressID = 0, AddressLine1 = "2 Main St", AddressLine2 = "Apt 1", City = "Bigcity", StateProvinceID = 79, PostalCode = "98745", AddressTypeID =2 },
            new (){ AddressID = 0, AddressLine1 = "1 Main St", AddressLine2 = "Apt 1", City = "BIGCity", StateProvinceID = 79, PostalCode = "98745", AddressTypeID =1 }
        ];
        command = command with { Addresses = addressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Addresses);
    }

    [Fact]
    public async void RuleFor_Addresses_CreatePersonCommandValidator_Line1EmptyString_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<AddressDto> addressDtos =
        [
            new (){ AddressID = 0, AddressLine1 = "", AddressLine2 = "Apt 1", City = "Bigcity", StateProvinceID = 79, PostalCode = "98745", AddressTypeID =1 }
        ];
        command = command with { Addresses = addressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public async void RuleFor_Addresses_CreatePersonCommandValidator_CityEmptyString_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<AddressDto> addressDtos =
        [
            new (){ AddressID = 0, AddressLine1 = "Main St", AddressLine2 = "Apt 1", City = "", StateProvinceID = 79, PostalCode = "98745", AddressTypeID =1 }
        ];
        command = command with { Addresses = addressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public async void RuleFor_Addresses_CreatePersonCommandValidator_StateCodeZero_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<AddressDto> addressDtos =
        [
            new (){ AddressID = 0, AddressLine1 = "Main", AddressLine2 = "Apt 1", City = "Bigcity", StateProvinceID = 0, PostalCode = "98745", AddressTypeID =1 }
        ];
        command = command with { Addresses = addressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public async void RuleFor_Addresses_CreatePersonCommandValidator_PostalCodeEmptyString_ShouldReturn_Failure()
    {
        // Arrange
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();

        List<AddressDto> addressDtos =
        [
            new (){ AddressID = 0, AddressLine1 = "Main", AddressLine2 = "Apt 1", City = "Bigcity", StateProvinceID = 79, PostalCode = "", AddressTypeID =1 }
        ];
        command = command with { Addresses = addressDtos };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveAnyValidationError();
    }
}
