using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TenJames.DtoGenerator;

[Generator]
public class DtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Collect class declarations that have the [GenerateDto] attribute
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax classDecl && classDecl.AttributeLists.Any(),
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static classDecl => classDecl.AttributeLists
                .SelectMany(attrList => attrList.Attributes)
                .Any(attr => attr.Name.ToString() == "GenerateDto"))
            .Collect();

        // Combine with compilation to get semantic models
        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations);

        var specifications = new List<ObjectSpecification>();

        // Register source output
        context.RegisterSourceOutput(compilationAndClasses,
            action: (spc, source) =>
            {
                var (compilation, classes) = source;

                foreach (var classDecl in classes)
                {
                    var model = compilation.GetSemanticModel(classDecl.SyntaxTree);
                    var symbol = model.GetDeclaredSymbol(classDecl);

                    if (symbol == null)
                    {
                        continue;
                    }

                    var spec = ObjectSpecificationParser.Parse(symbol);

                    GenerateDtoCode(spec, DtoType.Read, ref spc);
                    GenerateDtoCode(spec, DtoType.ReadDetail, ref spc);
                    GenerateDtoCode(spec, DtoType.Create, ref spc);
                    GenerateDtoCode(spec, DtoType.Update, ref spc);
                    GenerateMapperProfile(spec, symbol.ContainingNamespace.ToString(), ref spc);

                    specifications.Add(spec);
                }
            });

        context.RegisterPostInitializationOutput(cpc =>
        {
            GenerateServiceRegistration(specifications, ref cpc);
        });
    }


    private static void GenerateDtoCode(ObjectSpecification specification, DtoType type,
        ref SourceProductionContext spc)
    {
        var dtoSuffix = type switch
        {
            DtoType.Read => "ReadDto",
            DtoType.ReadDetail => "ReadDetailDto",
            DtoType.Create => "CreateDto",
            DtoType.Update => "UpdateDto",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
        var codeBuilder = new StringBuilder($@"// <auto-generated />
using System.ComponentModel.DataAnnotations;
namespace GeneratedDtos
{{
    public partial class {specification.Name}{dtoSuffix}
    {{
");

        foreach (var property in specification.Properties)
        {
            if (property.Visibility.HasFlag(type))
            {
                if (property.Summary != string.Empty)
                {
                    codeBuilder.Append($"\t\t{property.Summary.Replace("\n", "\n\t\t")}\n");
                }

                // for each attribute that is from System.ComponentModel.DataAnnotations copy it to the generated code
                foreach (var a in property.Attributes)
                {
                    if (a.AttributeClass?.ContainingNamespace.ToString() == "System.ComponentModel.DataAnnotations")
                    {
                        codeBuilder.Append($"\t\t[{a.AttributeClass.Name}(");
                        for (var i = 0; i < a.ConstructorArguments.Length; i++)
                        {
                            if (i > 0)
                            {
                                codeBuilder.Append(", ");
                            }
                            codeBuilder.Append($"{a.ConstructorArguments[i].Value}");
                        }
                        codeBuilder.Append(")]\n\t\t");
                    }
                }
                if (type is DtoType.Read or DtoType.ReadDetail)
                {
                    if (property.MapTo != null)
                    {
                        codeBuilder.Append($"public {property.MapTo.ToType} {property.Name} {{ get; set; }}\n");
                    }
                    else
                    {
                        codeBuilder.Append($"public {property.Type} {property.Name} {{ get; set; }}\n");
                    }
                }
                else
                {
                    if (property.MapFrom != null)
                    {
                        codeBuilder.Append($"public {property.MapFrom.ToType} {property.MapFrom} {{ get; set; }}\n");
                    }
                    else
                    {
                        codeBuilder.Append($"public {property.Type} {property.Name} {{ get; set; }}\n");
                    }
                }

                codeBuilder.AppendLine();
            }
        }

        codeBuilder.Append("\t}\n}\n");
        spc.AddSource($"{specification.Name}{dtoSuffix}.g.cs", codeBuilder.ToString());
    }


    private static void GenerateMapperProfile(ObjectSpecification specification, string entityNamespace, ref SourceProductionContext spc)
    {
        var codeBuilder = new StringBuilder("//<auto-generated />\n");
        codeBuilder.AppendLine("using AutoMapper;");
        codeBuilder.AppendLine("using GeneratedDtos;");
        codeBuilder.AppendLine($"using {entityNamespace};");
        codeBuilder.AppendLine($"public class {specification.Name}Profile : Profile");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine($"\tpublic {specification.Name}Profile()");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine("\t\tReadDtoMap();");
        codeBuilder.AppendLine("\t\tReadDetailDtoMap();");
        codeBuilder.AppendLine("\t\tCreateDtoMap();");
        codeBuilder.AppendLine("\t\tUpdateDtoMap();");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("\tprivate void ReadDtoMap()");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine($"\t\tCreateMap<{entityNamespace}.{specification.Name}, {specification.Name}ReadDto>()");
        foreach (var property in specification.Properties.Where(p => p.MapTo != null))
        {
            codeBuilder.AppendLine($"\t\t\t.ForMember(dest => dest.{property.Name}, opt => opt.MapFrom(src => {property.MapTo!.Function}))");
        }

        codeBuilder.AppendLine("\t;");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("\tprivate void ReadDetailDtoMap()");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine($"\t\tCreateMap<{entityNamespace}.{specification.Name}, {specification.Name}ReadDetailDto>()");
        foreach (var property in specification.Properties.Where(p => p.MapTo != null))
        {
            codeBuilder.AppendLine($"\t\t\t.ForMember(dest => dest.{property.Name}, opt => opt.MapFrom(src => {property.MapTo!.Function}))");
        }

        codeBuilder.AppendLine("\t;");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("\tprivate void CreateDtoMap()");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine($"\t\tCreateMap<{specification.Name}CreateDto, {entityNamespace}.{specification.Name}>()");
        foreach (var property in specification.Properties.Where(p => p.MapFrom != null))
        {
            codeBuilder.AppendLine($"\t\t\t.ForMember(dest => dest.{property.Name}, opt => opt.MapFrom(src => {property.MapFrom!.Function}))");
        }


        codeBuilder.AppendLine("\t;");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("\tprivate void UpdateDtoMap()");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine($"\t\tCreateMap<{specification.Name}UpdateDto, {entityNamespace}.{specification.Name}>()");
        foreach (var property in specification.Properties.Where(p => p.MapFrom != null))
        {
            codeBuilder.AppendLine($"\t\t\t.ForMember(dest => dest.{property.Name}, opt => opt.MapFrom(src => {property.MapFrom!.Function}))");
        }


        codeBuilder.AppendLine("\t;");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine("}");
        spc.AddSource($"{specification.Name}Profile.cs", codeBuilder.ToString());


    }

    private static void GenerateServiceRegistration(IEnumerable<ObjectSpecification> specifications, ref IncrementalGeneratorPostInitializationContext cpc)
    {
        var codeBuilder = new StringBuilder("//<auto-generated />\n");
        codeBuilder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        codeBuilder.AppendLine("using AutoMapper;");
        codeBuilder.AppendLine("using GeneratedDtos;");
        codeBuilder.AppendLine("namespace GeneratedServices;");
        codeBuilder.AppendLine("public static class ServiceRegistration");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine("\tpublic static void AddGeneratedServices(this IServiceCollection services)");
        codeBuilder.AppendLine("\t{");
        codeBuilder.AppendLine("\t\tservices.AddAutoMapper(typeof(ServiceRegistration).Assembly);");
        codeBuilder.AppendLine("\t}");
        codeBuilder.AppendLine("}");
        cpc.AddSource("ServiceRegistration.cs", codeBuilder.ToString());
    }
}