using System.Reflection;

namespace JWTPermissionBased.Application.Common.Enums;

public class EnumBase<TKey, TValue, TEnum> where TEnum : EnumBase<TKey, TValue, TEnum>
{
    private static readonly List<TEnum> _list = new();
    public TKey Key { get; }
    public TValue Value { get; }

    protected EnumBase(TKey key, TValue val)
    {
        Key = key;
        Value = val;

        var item = this as TEnum;
        List.Add(item!);
    }

    public static TEnum FromKey(TKey key)
    {
        return List.Single(item => EqualityComparer<TKey>.Default.Equals(item.Key, key));
    }

    public static TEnum FromValue(TValue value)
    {
        return List.Single(item => EqualityComparer<TValue>.Default.Equals(item.Value, value));
    }

    public override string ToString() => $"{Key}: {Value}";

    private static bool _invoked;

    public static List<TEnum> List
    {
        get
        {
            if (!_invoked)
            {
                _invoked = true;
                typeof(TEnum).GetProperties(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(p => p.PropertyType == typeof(TEnum))?.GetValue(null, null);
            }

            return _list;
        }
    }

    public static IDictionary<string, TEnum> Dictionary
    {
        get
        {
            var propertyInfos = typeof(TEnum).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(TEnum))
                .ToList();

            return propertyInfos.
                ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(null, null) as TEnum)!;
        }
    }
}