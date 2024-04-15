using FluentValidation;
using AWC.PersonData.API.Domain.Interfaces;
using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Application.Features.UpdatePerson;

public sealed class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
    private static readonly string[] _personTypes = ["SC", "IN", "SP", "EM", "VC", "GC"];

    public UpdatePersonCommandValidator(IPersonRepository repository)
    {
        RuleFor(person => person.BusinessEntityID).MustAsync(async (businessEntityId, _) =>
        {
            Result<bool> result = await repository.IsValidBusinessEntityId(businessEntityId);
            return result.Value;
        }).WithMessage("A person with the business entity id provided could not be found.");

        RuleFor(person => person.PersonType)
                                    .Must(personType => Array.Exists(_personTypes, element => element.Equals(personType, StringComparison.CurrentCultureIgnoreCase)))
                                    .WithMessage("Valid person types are: SC, IN, SP, EM, VC, GC.");

        RuleFor(person => person.NameStyle)
                                    .Must(nameStyle => nameStyle >= 0 && nameStyle <= 1)
                                    .WithMessage("Valid name styles are 0 (western) or 1 (eastern).");

        RuleFor(person => person.Title)
                                    .MaximumLength(8).WithMessage("Title cannot be longer than 8 characters");

        RuleFor(person => person.FirstName)
                                    .NotEmpty().WithMessage("person first name; this is required.")
                                    .MaximumLength(50).WithMessage("person first name cannot be longer than 50 characters");

        RuleFor(person => person.LastName)
                                    .NotEmpty().WithMessage("person last name; this is required.")
                                    .MaximumLength(50).WithMessage("person last name cannot be longer than 50 characters");

        RuleFor(person => person.MiddleName)
                                    .MaximumLength(50).WithMessage("person middle name cannot be longer than 50 characters");

        RuleFor(person => person).MustAsync(async (args, _) =>
        {
            Result<bool> result = await repository.IsNameUniqueForUpdate(args.BusinessEntityID, args.FirstName, args.MiddleName!, args.LastName);

            return result.Value;
        }).WithMessage("Duplicate person name detected.");


        RuleFor(person => person.Suffix)
                                    .MaximumLength(10).WithMessage("Suffix cannot be longer than 10 characters");

        RuleFor(person => person.EmailPromotion)
                                    .Must(emailPromo => emailPromo >= 0 && emailPromo <= 2)
                                    .WithMessage("Valid email promo codes are 0, 1, or 2.");

        RuleFor(person => person.EmailAddresses).NotEmpty().WithMessage("An email address is required.");

        RuleFor(person => person.EmailAddresses).Custom((args, context) =>
        {
            HashSet<string> hashSet = [];

            if (args.Any(r => !hashSet.Add(r.MailAddress!.ToUpper())))
            {
                context.AddFailure("Duplicate email addresses detected.");
            }
        });

        RuleForEach(person => person.EmailAddresses).ChildRules(email =>
        {
            email.RuleFor(x => x.EmailAddressID).GreaterThan(0);
            email.RuleFor(x => x.MailAddress).NotEmpty();

            email.RuleFor(x => x).MustAsync(async (email, _) =>
            {
                Result<bool> result = await repository.IsEmailUniqueForUpdate(email.EmailAddressID, email.MailAddress!);
                return result.Value;
            }).WithMessage("Performing this update would result in duplicate email addresses.");
        });

        RuleFor(person => person.Telephones).NotEmpty().WithMessage("A telephone number is required.");

        RuleForEach(person => person.Telephones).ChildRules(ph =>
        {
            ph.RuleFor(x => x.PhoneNumber).Matches("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$")
                                          .WithMessage("Invalid phone number; a valid number should 10 digits with no dashes.");
            ;
            ph.RuleFor(x => x.PhoneNumberTypeID).Must(phType => phType >= 1 && phType <= 3)
                                                .WithMessage("Valid phone number types are 1, 2, and 3 (cell, home, work).");
        });

        RuleFor(person => person.Telephones).Custom((args, context) =>
        {
            HashSet<int> hashSet = [];

            if (args.Any(r => !hashSet.Add(r.PhoneNumberTypeID)))
            {
                context.AddFailure("Duplicate phone number types detected.");
            }
        });

        RuleFor(person => person.Addresses).NotEmpty().WithMessage("An address is required.");

        RuleFor(person => person.Addresses).Custom((args, context) =>
        {
            HashSet<string> hashSet = [];

            if (args.Any(r => !hashSet.Add(r.ToString().ToUpper())))
            {
                context.AddFailure("Duplicate addresses detected.");
            }
        });

        RuleForEach(person => person.Addresses).ChildRules(addr =>
        {
            addr.RuleFor(x => x.AddressID).GreaterThan(0);
            addr.RuleFor(x => x.AddressLine1).NotEmpty()
                                                .WithMessage("Address line 1; this is required.")
                                             .MaximumLength(60)
                                                .WithMessage("Address line cannot be longer than 60 characters");
            addr.RuleFor(x => x.AddressLine2).MaximumLength(60)
                                                .WithMessage("Address line cannot be longer than 60 characters");
            addr.RuleFor(x => x.City).NotEmpty()
                                        .WithMessage("City is required.")
                                     .MaximumLength(30)
                                        .WithMessage("City cannot be longer than 30 characters");
            addr.RuleFor(x => x.PostalCode).NotEmpty()
                                               .WithMessage("A postal code is required.")
                                           .MaximumLength(15)
                                               .WithMessage("The postal code cannot be longer than 15 characters");

            addr.RuleFor(x => x.StateProvinceID).MustAsync(async (id, _) =>
            {
                Result<bool> result = await repository.IsValidStateProvinceId(id);
                return result.Value;
            }).WithMessage("Invalid state province id.");
        });
    }
}
