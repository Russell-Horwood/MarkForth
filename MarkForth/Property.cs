using System.Linq.Expressions;
using System.Reflection;
using MarkForth.Extensions.Linq.Expressions;
using MarkForth.Extensions.Reflection;

namespace MarkForth;

internal sealed class Property<TEntity, TValue>
    where TValue : class?
{

    #region Dependency Injection.

    /// <summary>
    /// Initializes a new instance of the <see cref="Property"/> class using
    /// the specified expression to identify a property on an instance of 
    /// <see cref="TEntity"/> to represent.
    /// </summary>
    internal Property(Expression<Func<TEntity, TValue>> expression)
    {
        this.expression = expression;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Property"/> class using
    /// the specified property to identify a property on an instance of 
    /// <see cref="TEntity"/> to represent.
    /// </summary>
    /// <param name="info">
    /// Identifies a property on <see cref="TEntity"/> that this class represents.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="info"/> describes a property that is declared a type other than <see cref="TEntity"/>.
    /// -or- <paramref name="info"/> described a property with a type other than <see cref="TValue"/>.
    /// </exception>
    internal Property(PropertyInfo info)
    {
        Type entityType = typeof(TEntity);
        if (info.DeclaringType != entityType)
        {
            throw new ArgumentException
            (
                $"The specified info describes a property that is not declared on type '{entityType.FullName}'.",
                nameof(info)
            );
        }

        Type valueType = typeof(TValue);
        if (info.PropertyType != valueType)
        {
            throw new ArgumentException
            (
                $"The specified info describes a property with a type other than '{valueType.FullName}'.",
                nameof(info)
            );
        }

        this.infoField = info;
    }

    #endregion Dependency Injection.

    #region info.

    private readonly Expression<Func<TEntity, TValue>>? expression;

    private PropertyInfo? infoField;

    private PropertyInfo info
    {
        get
        {
            if (this.infoField == null)
                this.infoField = this.expression?.GetPropertyInfo();

            return this.infoField!;
        }
    }

    #endregion info.

    /// <summary>
    /// Gets the name of the property represented by this class.
    /// </summary>
    internal string Name => this.info.Name;

    /// <summary>
    /// Gets a value that indicates if the property represented by this class is a nullable type.
    /// </summary>
    internal bool IsNullable => this.info.IsNullable();

    /// <summary>
    /// Get's the value of the property represented by this class
    /// from the specified entity.
    /// </summary>
    /// <param name="entity">
    /// The entity from which to get the property.
    /// </param>
    /// <returns>
    /// The property's value.
    /// </returns>
    internal TValue Get(TEntity entity)
    {
        return (this.info.GetValue(entity) as TValue)!;
    }

    /// <summary>
    /// Set's the value of the property represented by this class 
    /// on the specified entity,
    /// to the specified value.
    /// </summary>
    /// <param name="entity">
    /// The entity on which to set the property.
    /// </param>
    /// <param name="value">
    /// The value to set the property to.
    /// </param>
    /// <returns>
    /// <paramref name="entity"/>.
    /// </returns>
    /// <exception cref="TargetException">
    /// The property's set accessor is not found.
    /// </exception>
    /// <exception cref="MethodAccessException">
    /// There was an illegal attempt to access a private or protected method inside a class.
    /// Note: In .NET for Windows Store apps or the Portable Class Library,
    /// catch the base class exception, <see cref="MemberAccessException"/> instead.
    /// </exception>
    /// <exception cref="TargetInvocationException">
    /// An error occurred while setting the property value.
    /// The <see cref="Exception.InnerException"/> property indicates the reason for the error.
    /// </exception>
    internal TEntity Set(TEntity entity, TValue value)
    {
        this.info.SetValue(entity, value);
        return entity;
    }
}
