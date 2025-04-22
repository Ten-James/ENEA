using Api.Infrastructure.Enums;
using Api.Infrastructure.Models;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace Main_Api.Helpers;

internal static class ApiSpecificationHelper
{

    public static string StripOnlyDtoFromResponse(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>))
        {
            return StripOnlyDtoFromResponse(type.GenericTypeArguments[0]);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IActionResult))
        {
            return StripOnlyDtoFromResponse(type.GenericTypeArguments[0]);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return StripOnlyDtoFromResponse(type.GenericTypeArguments[0]);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            return StripOnlyDtoFromResponse(type.GenericTypeArguments[0]);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PaginationResponse<>))
        {
            return "PaginationResponse<" + StripOnlyDtoFromResponse(type.GenericTypeArguments[0]) + ">";
        }

        return type.Name;
    }



    private static IEnumerable<ApiModel> GetModels()
    {
        var models = typeof(EntityBase).Assembly.GetTypes().Where(x => x.Name.Contains("Dto")).Select(model => new ApiModel { Name = model.Name, Description = DtoJsonSpecificationGenerator.GenerateJsonSpecification(model) }).ToList();
        models.AddRange(typeof(LoginRequest).Assembly.GetTypes().Select(model => new ApiModel { Name = model.Name, Description = DtoJsonSpecificationGenerator.GenerateJsonSpecification(model) }));
        models.AddRange(typeof(ChargerStatus).Assembly.GetTypes().Where(x => x.IsEnum).Select(type => new ApiModel { Name = type.Name, Description = DtoJsonSpecificationGenerator.GenerateJsonSpecification(type) }));
        return models;
    }


    private static IEnumerable<ApiRoute> GetRoutes()
    {
        var routes = new List<ApiRoute>();

        foreach (var controller in typeof(ApiSpecificationHelper).Assembly.GetTypes()
                     .Where(t => t.Namespace == "Main_Api.Controllers" && t.Name.EndsWith("Controller")))
        {
            // for each method try to get the route
            var route = controller.GetCustomAttributes(typeof(RouteAttribute), true).FirstOrDefault() as RouteAttribute;
            foreach (var method in controller.GetMethods())
            {
                if (method.GetCustomAttributes(typeof(HttpMethodAttribute), true).FirstOrDefault() is HttpMethodAttribute methodAttribute)
                {
                    var requestBodyType = method.GetParameters()
                        .FirstOrDefault(p => p.GetCustomAttribute<FromBodyAttribute>() != null)?.ParameterType;

                    routes.Add(new ApiRoute
                    {
                        Name = method.Name,
                        Method = methodAttribute.HttpMethods.First(),
                        Path = (route!.Template.TrimEnd('/') + "/" + methodAttribute.Template?.TrimStart('/') ?? "").TrimEnd('/'),
                        RequestBody = requestBodyType?.Name ?? "",
                        ResponseBody = StripOnlyDtoFromResponse(method.ReturnType)
                    });
                }
            }
        }


        return routes;
    }

    internal static ApiSpecification GetApiSpecification()
    {
        var specification = new ApiSpecification { Name = "Enea-Api", Version = "v1" };

        specification.Routes = GetRoutes();

        specification.Models = GetModels();

        return specification;
    }

    internal struct ApiRoute
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryParameters { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }

    internal struct ApiModel
    {
        public string Name { get; set; }
        public object Description { get; set; }
    }


    internal struct ApiSpecification
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public IEnumerable<ApiRoute> Routes { get; set; }
        public IEnumerable<ApiModel> Models { get; set; }

    }
}