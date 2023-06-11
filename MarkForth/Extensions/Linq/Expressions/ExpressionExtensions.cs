using System.Linq.Expressions;
using System.Reflection;

namespace MarkForth.Extensions.Linq.Expressions;

internal static class ExpressionExtensions
{
    /// <summary>
    /// Gets an instance of <see cref="PropertyInfo"/> that represents
    /// a property of <paramref name="TSource"/> 
    /// that is specified by the supplied expression.
    /// </summary>
    /// <typeparam name="TSource">
    /// the type the property is of.
    /// </typeparam>
    /// <typeparam name="TProperty">
    /// The type of the property.
    /// </typeparam>
    /// <param name="expression">
    /// An expression that selects a property given it's type.
    /// </param>
    /// <returns>
    /// An instance of <see cref="PropertyInfo"/> that represents
    /// a property of <paramref name="TSource"/> 
    /// that is specified by the supplied expression.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="expression"/> does not refer to a property of <paramref name="TSource"/>.
    /// </exception>
    internal static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> expression)
    {
        MemberExpression? member = expression.Body as MemberExpression;
        if (member == null)
            throw new ArgumentException($"Expression '{expression}' refers to a method, not a property.");

        PropertyInfo? property = member.Member as PropertyInfo;
        if (property == null)
            throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");

        Type type = typeof(TSource);
        if (type != property.ReflectedType && (property.ReflectedType == null || !type.IsSubclassOf(property.ReflectedType)))
            throw new ArgumentException($"Expression '{expression}' refers to a property that is not from type {type}.");

        return property;
    }
}
