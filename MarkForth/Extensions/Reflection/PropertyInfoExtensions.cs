using System.Reflection;

namespace MarkForth.Extensions.Reflection;

internal static class PropertyInfoExtensions
{
    private static readonly NullabilityInfoContext nullabilityInfoContext = new NullabilityInfoContext();

    /// <summary>
    /// Determines if the specified property's value is a nullable type.
    /// </summary>
    /// <param name="propertyInfo">
    /// The information object that described the property.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the property's value is a nullable type,
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertyInfo"/> is <see langword="null"/>.
    /// </exception>
    internal static bool IsNullable(this PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return nullabilityInfoContext.Create(propertyInfo).WriteState == NullabilityState.Nullable;
    }
}
