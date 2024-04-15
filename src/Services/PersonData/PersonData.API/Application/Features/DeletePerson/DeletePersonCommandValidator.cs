using FluentValidation;
using AWC.PersonData.API.Domain.Interfaces;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Application.Features.DeletePerson;

public sealed class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonCommandValidator(IPersonRepository repository)
    {
        RuleFor(person => person.BusinessEntityID).MustAsync(async (businessEntityId, _) =>
        {
            Result<bool> result = await repository.IsValidBusinessEntityId(businessEntityId);
            return result.Value;
        }).WithMessage("A person with the business entity id provided could not be found.");
    }
}
