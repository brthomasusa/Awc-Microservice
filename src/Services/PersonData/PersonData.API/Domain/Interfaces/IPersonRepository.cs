using AWC.Shared.Kernel.Utilities;
using AWC.PersonData.API.Domain.PersonAggregate;

namespace AWC.PersonData.API.Domain.Interfaces;

public interface IPersonRepository
{
    Task<Result<Person>> GetByIdAsync(int id);
    Task<Result<Person>> GetByIdWithChildrenAsync(int id);
    Task<Result<int>> AddAsync(Person person);
    Task<Result> UpdateAsync(Person person);
    Task<Result> DeleteAsync(int id);
    Task<Result<bool>> IsNameUniqueForCreate(string firstName, string? middleInitial, string lastName);
    Task<Result<bool>> IsNameUniqueForUpdate(int id, string firstName, string? middleInitial, string lastName);
    Task<Result<bool>> IsEmailUniqueForCreate(string email);
    Task<Result<bool>> IsEmailUniqueForUpdate(int id, string email);
    Task<Result<bool>> IsValidStateProvinceId(int stateProvinceId);
    Task<Result<bool>> IsValidBusinessEntityId(int businessEntityId);
}
