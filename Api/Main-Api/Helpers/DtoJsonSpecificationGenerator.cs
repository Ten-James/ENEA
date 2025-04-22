using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Main_Api.Helpers;

internal static class DtoJsonSpecificationGenerator
{

    internal static object GenerateJsonSpecification<T>()
    {
        return GenerateJsonSpecification(typeof(T));
    }

    internal static object GenerateJsonSpecification(Type type)
    {
        if (type.IsEnum)
        {
            return type.GetEnumValues().Cast<object>().Select(v => new { Name = type.GetEnumName(v), Value = v });
        }

        return type.GetProperties()
            .Select(prop => new { prop.Name, Type = GetTypeName(prop.PropertyType), Summary = GetPropertySummary(prop) })
            .ToList();
    }

    internal static string GetTypeName(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return $"{Nullable.GetUnderlyingType(type)?.Name}?";
        }
        if (type.IsArray)
        {
            return $"{type.GetElementType()?.Name}[]";
        }
        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            var elementType = type.GetGenericArguments().FirstOrDefault() ?? typeof(object);
            return $"{elementType.Name}[]";
        }
        return type.Name;
    }

    internal static string GetPropertySummary(PropertyInfo prop)
    {
        var attr = prop.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? "";
    }
}