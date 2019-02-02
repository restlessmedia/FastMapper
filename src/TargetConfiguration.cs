using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FastMapper
{
  public class TargetConfiguration<TSource, TTarget> : TargetConfiguration<TTarget>
  {
    public TargetConfiguration(TargetConfiguration existingConfiguration)
      : base(existingConfiguration) { }

    public TargetConfiguration() { }

    public new MemberConfiguration<TSource, TTarget, TMember> For<TMember>(Expression<Func<TTarget, TMember>> expression)
    {
      MemberConfiguration<TSource, TTarget, TMember> memberConfiguration = new MemberConfiguration<TSource, TTarget, TMember>(expression);
      AddMemberConfiguration(memberConfiguration);
      return memberConfiguration;
    }
  }

  public class TargetConfiguration<TTarget> : TargetConfiguration
  {
    public TargetConfiguration(TargetConfiguration existingConfiguration)
      : base(existingConfiguration) { }

    public TargetConfiguration() { }

    public virtual CollectionMemberConfiguration<TTarget, TMember> ForEach<TMember>(Expression<Func<TTarget, TMember>> expression)
      where TMember : IEnumerable
    {
      if (typeof(TMember).GenericTypeArguments.Length == 0)
      {
        throw new NotSupportedException();
      }

      CollectionMemberConfiguration<TTarget, TMember> collectionMemberConfiguration = new CollectionMemberConfiguration<TTarget, TMember>(expression);
      AddMemberConfiguration(collectionMemberConfiguration);
      return collectionMemberConfiguration;
    }

    public virtual MemberConfiguration<TTarget, TMember> For<TMember>(Expression<Func<TTarget, TMember>> expression)
    {
      MemberConfiguration<TTarget, TMember> memberConfiguration = new MemberConfiguration<TTarget, TMember>(expression);
      AddMemberConfiguration(memberConfiguration);
      return memberConfiguration;
    }
  }

  public abstract class TargetConfiguration
  {
    public TargetConfiguration(TargetConfiguration existingConfiguration)
    {
      _configurations = existingConfiguration._configurations;
    }

    public TargetConfiguration()
    {
      _configurations = new Dictionary<MemberInfo, MemberConfiguration>(0);
    }

    internal void AddMemberConfiguration(MemberConfiguration memberConfiguration)
    {
      _configurations.Add(memberConfiguration.Member, memberConfiguration);
    }

    internal IEnumerable<TypeResolver> GetTypeResolvers()
    {
      foreach (MemberConfiguration memberConfiguration in _configurations.Values)
      {
        foreach (TypeResolver typeResolver in memberConfiguration.TypeResolvers)
        {
          yield return typeResolver;
        }
      }
    }

    internal IEnumerable<MemberMap> GetMemberMaps()
    {
      foreach (MemberConfiguration memberConfiguration in _configurations.Values)
      {
        foreach (MemberMap memberMap in memberConfiguration.Maps)
        {
          yield return memberMap;
        }
      }
    }

    private readonly Dictionary<MemberInfo, MemberConfiguration> _configurations;
  }
}