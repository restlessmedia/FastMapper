using FastMember;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FastMapper
{
  public class TargetConfiguration<TSource, TTarget> : TargetConfiguration<TTarget>
  {
    public TargetConfiguration(TargetConfiguration existingConfiguration)
      : base(existingConfiguration) { }

    public TargetConfiguration()
      : base() { }

    public new MemberConfiguration<TSource, TTarget, TMember> For<TMember>(Expression<Func<TTarget, TMember>> expression)
    {
      MemberConfiguration<TSource, TTarget, TMember> memberConfiguration = new MemberConfiguration<TSource, TTarget, TMember>(expression);

      if (TryGet(memberConfiguration, out MemberConfiguration existingMemberConfiguration))
      {
        return new MemberConfiguration<TSource, TTarget, TMember>(existingMemberConfiguration);
      }

      return memberConfiguration;
    }
  }


  public class TargetConfiguration<TTarget> : TargetConfiguration
  {
    public TargetConfiguration(TargetConfiguration existingConfiguration)
      : base(existingConfiguration) { }

    public TargetConfiguration()
      : base() { }

    public virtual CollectionMemberConfiguration<TTarget, TMember> ForEach<TMember>(Expression<Func<TTarget, TMember>> expression)
      where TMember : IEnumerable
    {
      if (typeof(TMember).GenericTypeArguments.Length == 0)
      {
        throw new NotSupportedException();
      }

      CollectionMemberConfiguration<TTarget, TMember> collectionMemberConfiguration = new CollectionMemberConfiguration<TTarget, TMember>(expression);

      if (TryGet(collectionMemberConfiguration, out MemberConfiguration existingMemberConfiguration))
      {
        return new CollectionMemberConfiguration<TTarget, TMember>(existingMemberConfiguration);
      }

      return collectionMemberConfiguration;
    }

    public virtual MemberConfiguration<TTarget, TMember> For<TMember>(Expression<Func<TTarget, TMember>> expression)
    {
      MemberConfiguration<TTarget, TMember> memberConfiguration = new MemberConfiguration<TTarget, TMember>(expression);

      if (TryGet(memberConfiguration, out MemberConfiguration existingMemberConfiguration))
      {
        return new MemberConfiguration<TTarget, TMember>(existingMemberConfiguration);
      }

      return memberConfiguration;
    }
  }

  public abstract class TargetConfiguration
  {
    public TargetConfiguration(TargetConfiguration existingConfiguration)
    {
      MemberConfigurations = existingConfiguration.MemberConfigurations;
    }

    public TargetConfiguration()
    {
      MemberConfigurations = new Dictionary<MemberInfo, MemberConfiguration>(0);
    }

    protected bool TryGet(MemberConfiguration memberConfiguration, out MemberConfiguration existingMemberConfiguration)
    {
      if (!MemberConfigurations.TryGetValue(memberConfiguration.Member, out existingMemberConfiguration))
      {
        MemberConfigurations.Add(memberConfiguration.Member, memberConfiguration);
        return false;
      }
      return true;
    }

    internal IEnumerable<TypeResolver> GetTypeResolvers()
    {
      foreach (MemberConfiguration memberConfiguration in MemberConfigurations.Values)
      {
        foreach (TypeResolver typeResolver in memberConfiguration.TypeResolvers)
        {
          yield return typeResolver;
        }
      }
    }

    internal IEnumerable<MemberMap> GetMemberMaps()
    {
      foreach (MemberConfiguration memberConfiguration in MemberConfigurations.Values)
      {
        foreach (MemberMap memberMap in memberConfiguration.Maps)
        {
          yield return memberMap;
        }
      }
    }

    protected readonly Dictionary<MemberInfo, MemberConfiguration> MemberConfigurations;
  }
}