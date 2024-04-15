using AWC.PersonData.API.Application.Features.CreatePerson;
using AWC.PersonData.API.Application.Features.DeletePerson;
using AWC.PersonData.API.Application.Features.GetByIdWithChildren;
using AWC.PersonData.API.Application.Features.UpdatePerson;
using AWC.PersonData.API.Infrastructure.Persistence.Dtos;
using AWC.PersonData.API.Web.Extensions;
using AWC.Shared.Kernel.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWC.PersonData.API.Web.Endpoints;

public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/people");

        group.MapGet("getbyid/{id}", GetPersonById);
        group.MapPost("add", AddPerson);
        group.MapPut("edit", EditPerson);
        group.MapDelete("delete/{id}", DeletePerson);
    }

    public static async Task<IResult> GetPersonById(int id, ISender sender)
    {
        Result<PersonByIdWithChildrenDto>? result = null;

        try
        {
            var query = new GetByIdWithChildrenQuery(id);
            result = await sender.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToNotFoundProblemDetails();
        }
        catch (Exception ex)
        {
            return result!.ToInternalServerErrorProblemDetails(ex.Message);
        }
    }

    public static async Task<IResult> AddPerson([FromBody] CreatePersonCommand command, ISender sender)
    {
        Result<int>? result = null;

        try
        {
            result = await sender.Send(command);

            if (result.IsSuccess)
            {
                var query = new GetByIdWithChildrenQuery(result.Value);
                Result<PersonByIdWithChildrenDto>? getResult = await sender.Send(query);
                return Results.Created($"getbyid/{result.Value}", getResult.Value);
            }

            return result.ToBadRequestProblemDetails();
        }
        catch (Exception ex)
        {
            return result!.ToInternalServerErrorProblemDetails(ex.Message);
        }
    }

    public static async Task<IResult> EditPerson(UpdatePersonCommand command, ISender sender)
    {
        Result result = await sender.Send(command);

        return result.IsSuccess ? Results.Ok() : result.ToBadRequestProblemDetails();
    }

    public static async Task<IResult> DeletePerson(int id, ISender sender)
    {
        DeletePersonCommand command = new(BusinessEntityID: id);
        Result? result = null;

        try
        {
            result = await sender.Send(command);

            return result.IsSuccess ? Results.Ok() : result.ToBadRequestProblemDetails();
        }
        catch (Exception ex)
        {
            return result!.ToInternalServerErrorProblemDetails(ex.Message);
        }
    }
}
