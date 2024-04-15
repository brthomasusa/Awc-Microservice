using AWC.PersonData.API;
using Mapster;
using MapsterMapper;

namespace PersonData.UnitTests;

public static class AddMapsterForUnitTests
{
    public static Mapper GetMapper()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(
            typeof(ServerAssembly).Assembly
        );

        config.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);

        return new Mapper(config);
    }
}
