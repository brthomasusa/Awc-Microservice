using AWC.PersonData.API.Domain.PersonAggregate;
using AWC.PersonData.API.Domain.PersonAggregate.EntityIDs;
using AWC.PersonData.API.Domain.PersonAggregate.Enums;
using AWC.PersonData.API.Domain.PersonAggregate.ValueObjects;
using AWC.Shared.Kernel.Base;
using AWC.Shared.Kernel.Utilities;
using PersonData.UnitTests.Data;

namespace PersonData.UnitTests.Domain.PersonAggregate;

public class PersonAggregateTests
{
    [Fact]
    public void Address_Create_ValidData_ShouldReturn_Success()
    {
        // Arrange

        // Act
        Result<Address> result = Address.Create(new AddressID(1), AddressType.Home, "123 Main Street", "Apt 1", "Dallas", 73, "75231");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Address_Create_InvalidData_NullLine1_ShouldReturn_Failure()
    {
        // Arrange

        // Act
        Result<Address> result = Address.Create(new AddressID(1), AddressType.Home, null!, "Apt 1", "Dallas", 73, "75231");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Address_Update_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Address> address = Address.Create(new AddressID(1), AddressType.Home, "123 Main Street", "Apt 1", "Dallas", 73, "75231");

        // Act
        Result<Address> result = address.Value.Update(AddressType.Home, "123 Main Street", "Apt 1", "Dallas", 73, "75231");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Address_Update_InvalidData_NullLine1_ShouldReturn_Failure()
    {
        // Arrange
        Result<Address> address = Address.Create(new AddressID(1), AddressType.Home, "123 Main Street", "Apt 1", "Dallas", 73, "75231");

        // Act
        Result<Address> result = address.Value.Update(AddressType.Home, null!, "Apt 1", "Dallas", 73, "75231");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void PersonEmailAddress_Create_ValidData_ShouldReturn_Success()
    {
        // Arrange

        // Act
        Result<PersonEmailAddress> emailAddress = PersonEmailAddress.Create(new PersonEmailAddressID(1), "j.doe@adventureworks.com");

        // Assert
        Assert.True(emailAddress.IsSuccess);
    }

    [Fact]
    public void PersonEmailAddress_Create_InvalidData_NullEmail_ShouldReturn_Failure()
    {
        // Arrange

        // Act
        Result<PersonEmailAddress> emailAddress = PersonEmailAddress.Create(new PersonEmailAddressID(1), null!);

        // Assert
        Assert.True(emailAddress.IsFailure);
    }

    [Fact]
    public void PersonEmailAddress_Update_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<PersonEmailAddress> emailAddress = PersonEmailAddress.Create(new PersonEmailAddressID(1), "j.doe@adventureworks.com");

        // Act
        Result<PersonEmailAddress> result = emailAddress.Value.UpdateEmailAddress("j.doe@adventureworks.com");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void PersonEmailAddress_Update_InvalidData_NullEmailAddress_ShouldReturn_Failure()
    {
        // Arrange
        Result<PersonEmailAddress> emailAddress = PersonEmailAddress.Create(new PersonEmailAddressID(1), "j.doe@adventureworks.com");

        // Act
        Result<PersonEmailAddress> result = emailAddress.Value.UpdateEmailAddress(null!);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void PersonPhone_Create_ValidData_ShouldReturn_Success()
    {
        // Arrange

        // Act
        Result<PersonPhone> phoneResult = PersonPhone.Create(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Assert
        Assert.True(phoneResult.IsSuccess);
    }

    [Fact]
    public void PersonPhone_Create_InvalidData_NullPhoneNumber_ShouldReturn_Failure()
    {
        // Arrange

        // Act
        Result<PersonPhone> phoneResult = PersonPhone.Create(new PersonPhoneID(1), PhoneNumberType.Home, null!);

        // Assert
        Assert.True(phoneResult.IsFailure);
    }

    [Fact]
    public void PersonPhone_Update_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<PersonPhone> phoneResult = PersonPhone.Create(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Act
        Result<PersonPhone> result = phoneResult.Value.Update(PhoneNumberType.Cell, "214-878-0011");

        // Assert
        Assert.True(phoneResult.IsSuccess);
    }

    [Fact]
    public void PersonPhone_Update_InvalidData_ShouldReturn_Failure()
    {
        // Arrange
        Result<PersonPhone> phoneResult = PersonPhone.Create(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Act
        Result<PersonPhone> result = phoneResult.Value.Update(PhoneNumberType.Cell, null!);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_AddAddress_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();

        // Act
        Result<Address> result = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EntityStatus.Added, result.Value.EntityStatus);
    }

    [Fact]
    public void Person_AddAddress_ValidData_OnlyAddressTypeIsDifferent_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address = person.Value.AddAddress(new AddressID(0), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Act
        Result<Address> result = person.Value.AddAddress(new AddressID(0), AddressType.Archive, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EntityStatus.Added, result.Value.EntityStatus);
    }

    [Fact]
    public void Person_AddAddress_InvalidData_NonLine1_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();

        // Act
        Result<Address> result = person.Value.AddAddress(new AddressID(1), AddressType.Home, null!, "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_AddAddress_InvalidData_DupeNonZeroAddressID_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address1 = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Act
        Result<Address> result = person.Value.AddAddress(new AddressID(1), AddressType.Home, "321 Main", "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_AddAddress_InvalidData_DupeAddress_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address1 = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Act
        Result<Address> result = person.Value.AddAddress(new AddressID(2), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_AddAddress_InvalidData_DupeNewAddresses_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address1 = person.Value.AddAddress(new AddressID(0), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Act
        Result<Address> result = person.Value.AddAddress(new AddressID(0), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_UpdateAddress_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");
        address = person.Value.AddAddress(new AddressID(2), AddressType.Home, "123 Main", "Apt 2", "Austin", 73, "78880");

        // Act
        Result<Address> result = person.Value.UpdateAddress(new AddressID(2), AddressType.Home, "123 1st Street", "Apt 2", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_UpdateAddress_InvalidData_BadAddressID_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");
        address = person.Value.AddAddress(new AddressID(2), AddressType.Home, "123 Main", "Apt 2", "Austin", 73, "78880");

        // Act
        Result<Address> result = person.Value.UpdateAddress(new AddressID(3), AddressType.Home, "123 1st Street", "Apt 2", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_UpdateAddress_InvalidData_DuplicateAddress_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address1 = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");
        address1.Value.EntityStatus = EntityStatus.Modified;

        Result<Address> address2 = person.Value.AddAddress(new AddressID(2), AddressType.Home, "123 1st Street", "Apt 2", "Austin", 73, "78880");
        address2.Value.EntityStatus = EntityStatus.Modified;


        // Act
        Result<Address> result = person.Value.UpdateAddress(new AddressID(2), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_UpdateAddress_InvalidData_MeaninglessUpdate_ShouldReturn_Success()
    {
        // A meaningless update is updating an address with the values it already has
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");
        address = person.Value.AddAddress(new AddressID(2), AddressType.Home, "123 1st Street", "Apt 2", "Austin", 73, "78880");

        // Act
        Result<Address> result = person.Value.UpdateAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_DeleteAddress_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> person = PersonTestData.GetValidPersonAggregate();
        Result<Address> address = person.Value.AddAddress(new AddressID(1), AddressType.Home, "123 Main", "Apt 1", "Austin", 73, "78880");

        // Act
        Result result = person.Value.DeleteAddress(new AddressID(1));

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_AddPersonEmailAddress_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();

        // Act
        Result<PersonEmailAddress> result = Person.Value.AddEmailAddress(new PersonEmailAddressID(0), @"billy.bob@adventureworks.com");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_AddPersonEmailAddress_InvalidData_DuplicateID_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonEmailAddress> email1 = Person.Value.AddEmailAddress(new PersonEmailAddressID(1), @"billy.bob@adventureworks.com");

        // Act
        Result<PersonEmailAddress> email2 = Person.Value.AddEmailAddress(new PersonEmailAddressID(1), @"silly.bob@adventureworks.com");

        // Assert
        Assert.True(email2.IsFailure);
    }

    [Fact]
    public void Person_AddPersonEmailAddress_InvalidData_DuplicateEmailAddress_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonEmailAddress> email1 = Person.Value.AddEmailAddress(new PersonEmailAddressID(1), @"billy.bob@adventureworks.com");

        // Act
        Result<PersonEmailAddress> email2 = Person.Value.AddEmailAddress(new PersonEmailAddressID(2), @"BILLY.bOb@adventureworks.com");

        // Assert
        Assert.True(email2.IsFailure);
    }

    [Fact]
    public void Person_DeletePersonEmailAddress_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonEmailAddress> email1 = Person.Value.AddEmailAddress(new PersonEmailAddressID(1), @"billy.bob@adventureworks.com");

        // Act
        Result result = Person.Value.DeleteEmailAddress(new PersonEmailAddressID(1));

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_DeletePersonEmailAddress_InvalidData_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonEmailAddress> email1 = Person.Value.AddEmailAddress(new PersonEmailAddressID(1), @"billy.bob@adventureworks.com");

        // Act
        Result result = Person.Value.DeleteEmailAddress(new PersonEmailAddressID(11));

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_AddPersonPhone_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();

        // Act
        Result<PersonPhone> result = Person.Value.AddPhoneNumber(new PersonPhoneID(2), PhoneNumberType.Home, "555-555-5555");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_AddPersonPhone_ValidData_DuplicateZeroID_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonPhone> result = Person.Value.AddPhoneNumber(new PersonPhoneID(0), PhoneNumberType.Home, "555-555-5555");

        // Act
        result = Person.Value.AddPhoneNumber(new PersonPhoneID(0), PhoneNumberType.Home, "214-555-5555");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_AddPersonPhone_InvalidData_DuplicatePhoneNumber_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonPhone> result = Person.Value.AddPhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Act
        result = Person.Value.AddPhoneNumber(new PersonPhoneID(0), PhoneNumberType.Home, "555-555-5555");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_AddPersonPhone_InvalidData_DuplicateNonZeroID_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonPhone> result = Person.Value.AddPhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Act
        result = Person.Value.AddPhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "214-555-5555");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_UpdatePersonPhone_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonPhone> result = Person.Value.AddPhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Act
        result = Person.Value.UpdatePhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "817-555-5555");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_UpdatePersonPhone_InvalidData_DuplicatePhoneNumber_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonPhone> result = Person.Value.AddPhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");
        result = Person.Value.AddPhoneNumber(new PersonPhoneID(2), PhoneNumberType.Cell, "817-555-5555");

        // Act
        result = Person.Value.UpdatePhoneNumber(new PersonPhoneID(1), PhoneNumberType.Cell, "817-555-5555");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Person_DeletePersonPhone_ValidData_ShouldReturn_Success()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonPhone> telephone = Person.Value.AddPhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Act
        Result result = Person.Value.DeletePhoneNumber(new PersonPhoneID(1));

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Person_DeletePersonPhone_InvalidData_InvalidID_ShouldReturn_Failure()
    {
        // Arrange
        Result<Person> Person = PersonTestData.GetValidPersonAggregate();
        Result<PersonPhone> telephone = Person.Value.AddPhoneNumber(new PersonPhoneID(1), PhoneNumberType.Home, "555-555-5555");

        // Act
        Result result = Person.Value.DeletePhoneNumber(new PersonPhoneID(11));

        // Assert
        Assert.True(result.IsFailure);
    }
}
