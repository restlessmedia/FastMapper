﻿using System;

namespace FastMapper
{
  public abstract class ValueBinder<T> : ValueBinder
  {
    public ValueBinder(Configuration configuration)
      : base(configuration) { }

    public override bool CanBind(Type targetMemberType)
    {
      return targetMemberType == typeof(T);
    }
  }

  public abstract class ValueBinder
  {
    public ValueBinder(Configuration configuration)
    {
      Configuration = configuration;
    }

    public abstract bool CanBind(Type targetMemberType);

    public abstract void Bind(ITypeAccessor targetAccessor, IMember targetMember, ValueBinderContext valueBinderContext);

    /// <summary>
    /// Resolves the source name from either the mapping configuration or target member.
    /// </summary>
    /// <param name="targetMember"></param>
    /// <param name="targetConfiguration"></param>
    /// <returns></returns>
    protected static string GetSourceNameOrDefault(IMember targetMember, TargetConfiguration targetConfiguration)
    {
      if (targetConfiguration != null)
      {
        foreach (MemberMap memberMap in targetConfiguration.GetMemberMaps())
        {
          if (memberMap.TargetMemberName == targetMember.Name)
          {
            return memberMap.SourceMemberName;
          }
        }
      }

      return targetMember.Name;
    }

    protected static Type FindResolvedType(Type type, TargetConfiguration targetConfiguration)
    {
      if (targetConfiguration == null)
      {
        return null;
      }

      foreach (TypeResolver typeResolver in targetConfiguration.GetTypeResolvers())
      {
        if (typeResolver.Type == type)
        {
          return typeResolver.ResolvedType;
        }
      }

      return null;
    }

    /// <summary>
    /// Try and find a resolved type for the given type and target configuration.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="targetConfiguration"></param>
    /// <param name="resolvedType"></param>
    /// <returns></returns>
    protected static bool TryFindResolvedType(Type type, TargetConfiguration targetConfiguration, out Type resolvedType)
    {
      resolvedType = FindResolvedType(type, targetConfiguration);
      return resolvedType != null;
    }

    /// <summary>
    /// Returns true if we can map to the specific type i.e it's not a string or primative type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected bool IsMappableType(Type type)
    {
      if (type.IsPrimitive)
      {
        return false;
      }

      if (type == typeof(string))
      {
        return false;
      }

      return type.IsClass;
    }

    protected readonly Configuration Configuration;
  }
}