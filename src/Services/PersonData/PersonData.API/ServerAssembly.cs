using System.Reflection;

namespace AWC.PersonData.API;

public static class ServerAssembly
{
    public static readonly Assembly Instance = typeof(ServerAssembly).Assembly;
}
