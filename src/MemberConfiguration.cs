using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FastMapper
{
  public class MemberConfiguration<TSource, TTarget, TMember> : MemberConfiguration<TTarget, TMember>
  {
    public MemberConfiguration(Expression<Func<TTarget, TMember>> expression)
      : base(expression) { }

    public MemberConfiguration(MemberConfiguration existingMemberConfiguration)
      : base(existingMemberConfiguration) { }

    public void MapFrom(string sourceMemberName)
    {
      Maps.Add(new MemberMap(sourceMemberName, Member));
    }

    public void MapFrom<TMapToMember>(Expression<Func<TSource, TMapToMember>> expression)
    {
      MemberInfo member = GetMember(expression);
      MapFrom(member.Name);
    }
  }

  public class MemberConfiguration<TTarget, TMember> : MemberConfiguration
  {
    public MemberConfiguration(Expression<Func<TTarget, TMember>> expression)
      : base(GetMember(expression)) { }

    public MemberConfiguration(MemberConfiguration existingMemberConfiguration)
      : base(existingMemberConfiguration) { }

    public virtual TypeResolver<TMember, TResolvedWith> ResolveWith<TResolvedWith>()
    {
      TypeResolver<TMember, TResolvedWith> typeResolver = new TypeResolver<TMember, TResolvedWith>();
      TypeResolvers.Add(typeResolver);
      return typeResolver;
    }

    protected static MemberInfo GetMember<T, TProp>(Expression<Func<T, TProp>> expression)
    {
      MemberExpression memberExpression = expression.Body as MemberExpression;

      if (memberExpression == null)
      {
        throw new ArgumentException($"Expression '{expression}' refers to a method, not a property or field.");
      }

      MemberInfo member = memberExpression.Member as MemberInfo;

      if (member == null)
      {
        throw new ArgumentException($"Expression '{expression}' is not a member.");
      }

      return member;
    }
  }

  public class MemberConfiguration
  {
    public MemberConfiguration(MemberConfiguration existingMemberConfiguration)
    {
      Member = existingMemberConfiguration.Member;
      TypeResolvers = existingMemberConfiguration.TypeResolvers;
      Maps = existingMemberConfiguration.Maps;
    }

    public MemberConfiguration(MemberInfo member)
    {
      Member = member;
      TypeResolvers = new List<TypeResolver>(0);
      Maps = new List<MemberMap>(0);
    }

    internal readonly MemberInfo Member;

    internal readonly List<TypeResolver> TypeResolvers;

    internal readonly List<MemberMap> Maps;
  }
}