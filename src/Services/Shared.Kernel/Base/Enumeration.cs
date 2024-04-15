using System.Reflection;

namespace AWC.Shared.Kernel.Base;

public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<Dictionary<int, TEnum>> EnumerationsDictionary =
        new(() => CreateEnumerationDictionary(typeof(TEnum)));

    protected Enumeration(int id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    protected Enumeration()
    {
        Name = string.Empty;
    }


    public int Id { get; protected init; }

    public string Name { get; protected init; } = string.Empty;

    public static bool operator ==(Enumeration<TEnum>? a, Enumeration<TEnum>? b) => a is null && b is null ? true : a is null || b is null ? false : a.Equals(b);

    public static bool operator !=(Enumeration<TEnum> a, Enumeration<TEnum> b) => !(a == b);

    public static IReadOnlyCollection<TEnum> GetValues() => EnumerationsDictionary.Value.Values.ToList();

    public static TEnum? FromId(int id) => EnumerationsDictionary.Value.TryGetValue(id, out TEnum? enumeration) ? enumeration : null;

    public static TEnum? FromName(string name) => EnumerationsDictionary.Value.Values.SingleOrDefault(x => x.Name == name);

    public static bool Contains(int id) => EnumerationsDictionary.Value.ContainsKey(id);

    /// <inheritdoc />
    public virtual bool Equals(Enumeration<TEnum>? other)
    {
        return other is null ? false : GetType() == other.GetType() && other.Id.Equals(Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is null ? false : GetType() != obj.GetType() ? false : obj is Enumeration<TEnum> otherValue && otherValue.Id.Equals(Id);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode() * 37;

    private static Dictionary<int, TEnum> CreateEnumerationDictionary(Type enumType) => GetFieldsForType(enumType).ToDictionary(t => t.Id);

    private static IEnumerable<TEnum> GetFieldsForType(Type enumType) =>
        enumType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => enumType.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);
}
