using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace Main_Api.Helpers;

internal static class DtoJsonSpecificationGenerator
{

    internal static string GenerateJsonSpecification<T>()
    {
        return GenerateJsonSpecification(typeof(T));
    }

    internal static string GenerateJsonSpecification(Type type)
    {
        var properties = type.GetProperties().Select(prop => new { prop.Name, Type = GetTypeName(prop.PropertyType), Description = GetPropertySummary(prop) }).ToList();

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(properties, options);
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
            Type elementType = type.GetGenericArguments().FirstOrDefault() ?? typeof(object);
            return $"{elementType.Name}[]";
        }
        return type.Name;
    }

    internal static string GetPropertySummary(PropertyInfo prop)
    {
        DescriptionAttribute? attr = prop.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? "";
    }
}