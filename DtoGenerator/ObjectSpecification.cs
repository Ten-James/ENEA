using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TenJames.DtoGenerator;

internal struct ObjectSpecification
{
    internal DtoType DtoType { get; set; }
    internal string Name { get; set; }
    internal List<PropertySpecification> Properties { get; set; }
}

internal struct PropertySpecification
{
    internal string Name { get; set; }
    internal string Type { get; set; }
    internal string Summary { get; set; }
    internal ImmutableArray<AttributeData> Attributes { get; set; }
    internal bool IsNullable { get; set; }
    internal MappingSpecification? MapTo { get; set; }
    internal MappingSpecification? MapFrom { get; set; }
    internal DtoType Visibility { get; set; }
}

internal class MappingSpecification
{
    internal string ToType { get; set; }
    internal string Function { get; set; }
}