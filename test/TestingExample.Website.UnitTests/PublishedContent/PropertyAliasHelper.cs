using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.ModelsBuilder;

namespace TestingExample.Website.UnitTests.PublishedContent;

internal static class PropertyAliasHelper
{
    public static string AliasOf<TContent, TProp>(Expression<Func<TContent, TProp?>> expression)
        where TContent : IPublishedElement
    {
        if (expression.Body is not MemberExpression member)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.InvariantCulture,
                "Expression '{0}' refers to a method, not a property.",
                expression.ToString()));
        }

        if (member.Member is not PropertyInfo propInfo)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.InvariantCulture,
                "Expression '{0}' refers to a field, not a property.",
                expression.ToString()));
        }

        Type type = typeof(TContent);
        if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
        {
            throw new ArgumentException(string.Format(
                CultureInfo.InvariantCulture,
                "Expression '{0}' refers to a property that is not from type {1}.",
                expression.ToString(),
                type));
        }

        return propInfo.GetCustomAttribute<ImplementPropertyTypeAttribute>()!.Alias;
    }
}
