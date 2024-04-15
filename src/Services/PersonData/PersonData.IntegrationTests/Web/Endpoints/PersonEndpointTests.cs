#pragma warning disable CS8600, CS8602

using System.Net.Http.Headers;
using System.Text.Json;
using AWC.PersonData.API.Application.Features.CreatePerson;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;
using AWC.PersonData.API.Application.Features.GetByIdWithChildren;
using AWC.PersonData.API.Application.Features.UpdatePerson;
using PersonData.IntegrationTests.Data;

namespace PersonData.IntegrationTests.Web.Endpoints;

[Collection("Database Test")]
public class PersonEndpointTests(ApiWebApplicationFactory fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task Person_GetById_ShouldSucceed()
    {
        const int businessEntityId = 4;
        using var response = await _client.GetAsync($"{_urlRoot}getbyid/{businessEntityId}",
                                                    HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStreamAsync();
        var person = await JsonSerializer.DeserializeAsync<PersonByIdWithChildrenDto>(jsonResponse, _options);

        Assert.Equal("Rob", person.FirstName);
        Assert.Equal("Walters", person.LastName);
    }

    [Fact]
    public async Task Person_CreatePerson_ShouldSucceed()
    {
        string uri = $"{_urlRoot}add";
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStreamAsync();
        var person = await JsonSerializer.DeserializeAsync<PersonByIdWithChildrenDto>(jsonResponse, _options);
        Assert.Equal("Johnny", person.FirstName);
        Assert.Equal("Doe", person.LastName);
    }

    [Fact]
    public async Task Person_CreatePerson_DuplicateName_ShouldFail()
    {
        string uri = $"{_urlRoot}add";
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        command = command with { FirstName = "Rob", LastName = "Walters", MiddleName = null };

        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Person_CreatePerson_DuplicateEmail_ShouldFail()
    {
        string uri = $"{_urlRoot}add";
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        List<EmailAddressDto> emailAddressDtos =
        [
            new (){EmailAddressID = 0, MailAddress = "rob0@adventure-works.com"},
        ];
        command = command with { EmailAddresses = emailAddressDtos };

        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Person_CreatePerson_InvalidStateProvinceId_ShouldFail()
    {
        string uri = $"{_urlRoot}add";
        CreatePersonCommand command = PersonTestData.GetCreatePesonCommand();
        List<AddressDto> addressDtos =
        [
            new (){ AddressID = 0, AddressLine1 = "1 Main St", AddressLine2 = "Apt 1", City = "BIGCity", StateProvinceID = 199, PostalCode = "98745", AddressTypeID =1 }
        ];
        command = command with { Addresses = addressDtos };

        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Person_UpdatePerson_ShouldSucceed()
    {
        string uri = $"{_urlRoot}edit";
        UpdatePersonCommand command = PersonTestData.GetUpdatePesonCommand();
        command = command with { FirstName = "Bo", LastName = "Didley" };

        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Put, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Person_UpdatePerson_InvalidBusinessEntityId_ShouldFail()
    {
        string uri = $"{_urlRoot}edit";
        UpdatePersonCommand command = PersonTestData.GetUpdatePesonCommand();
        command = command with { BusinessEntityID = 2000 };

        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Put, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Person_UpdatePerson_DuplicateName_ShouldFail()
    {
        string uri = $"{_urlRoot}edit";
        UpdatePersonCommand command = PersonTestData.GetUpdatePesonCommand();
        command = command with { FirstName = "Rob", MiddleName = null, LastName = "Walters" };

        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Put, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Person_UpdatePerson_DuplicateEmails_ShouldFail()
    {
        string uri = $"{_urlRoot}edit";
        UpdatePersonCommand command = PersonTestData.GetUpdatePesonCommand();
        List<EmailAddressDto> emailAddressDtos =
        [
            new (){EmailAddressID = 16, MailAddress = "rob0@adventure-works.com"},
        ];
        command = command with { EmailAddresses = emailAddressDtos };

        var memStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memStream, command);
        memStream.Seek(0, SeekOrigin.Begin);

        var request = new HttpRequestMessage(HttpMethod.Put, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var requestContent = new StreamContent(memStream);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        Assert.False(response.IsSuccessStatusCode);
    }





}
