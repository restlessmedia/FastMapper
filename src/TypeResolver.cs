using System;

namespace FastMapper
{
  public class TypeResolver
  {
    public TypeResolver(Type type, Type resolvedType)
    {
      Type = type;
      ResolvedType = resolvedType;
    }

    public readonly Type Type;

    public readonly Type ResolvedType;
  }

  public class TypeResolver<TType, TResolvedType> : TypeResolver
  {
    public TypeResolver()
      : this(typeof(TType), typeof(TResolvedType)) { }

    public TypeResolver(Type type, Type resolvedType)
      : base(type, resolvedType) { }
  }
}