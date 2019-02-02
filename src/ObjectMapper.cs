using FastMember;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FastMapper
{
  public class ObjectMapper : IObjectMapper
  {
    public ObjectMapper(Configuration configuration)
    {
      _configuration = configuration;
    }

    public object Map(Type type, object source, TargetConfiguration targetConfiguration = null)
    {
      ValueProvider valueProvider = _configuration.ValueProviders.First(x => x.CanRead(source));
      return Map(type, source, valueProvider, targetConfiguration);
    }

    public object Map(Type type, object source, ValueProvider valueProvider, TargetConfiguration targetConfiguration)
    {
      if (source == null)
      {
        return null;
      }

      TypeAccessor targetAccessor = TypeAccessor.Create(type);
      object result = targetAccessor.CreateNew();
      ValueBinderContext valueBinderContext = new ValueBinderContext(source, result, targetConfiguration, valueProvider, this);

      foreach (Member targetMember in targetAccessor.GetMembers().Where(x => x.CanWrite))
      {
        ValueBinder valueBinder = _configuration.ValueBinders.First(x => x.CanBind(targetMember.Type));
        valueBinder.Bind(targetAccessor, targetMember, valueBinderContext);
      }

      return result;
    }

    public IEnumerable MapAll(Type type, IEnumerable source, TargetConfiguration targetConfiguration = null)
    {
      foreach (object obj in source)
      {
        yield return Map(type, obj, targetConfiguration);
      }
    }

    public static void Init(Action<Configuration> action)
    {
      Configuration configuration = Configuration.Current;
      action(configuration);

      // pre-load
      if (!configuration.LazyLoad)
      {
        foreach (Type targetType in configuration.TargetConfigurations.Keys)
        {
          foreach (TypeResolver typeResolver in configuration.TargetConfigurations[targetType].GetTypeResolvers())
          {
            // preload the resolved types
            TypeAccessor.Create(typeResolver.ResolvedType);
          }

          // preload the target types
          TypeAccessor.Create(targetType);
        }
      }
    }

    /// <summary>
    /// Map with optional configuration.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static TTarget Map<TSource, TTarget>(TSource source, Action<TargetConfiguration<TSource, TTarget>> targetConfigurationFactory = null)
    {
      if (source == null)
      {
        return default(TTarget);
      }

      TargetConfiguration<TSource, TTarget> targetConfiguration = null;

      if (targetConfigurationFactory != null)
      {
        Type targetType = typeof(TTarget);
        TargetConfiguration existingConfiguration;

        if (Configuration.Current.TargetConfigurations.TryGetValue(targetType, out existingConfiguration))
        {
          targetConfiguration = new TargetConfiguration<TSource, TTarget>(existingConfiguration);
        }
        else
        {
          targetConfiguration = new TargetConfiguration<TSource, TTarget>();
          Configuration.Current.TargetConfigurations[targetType] = targetConfiguration;
        }

        targetConfigurationFactory(targetConfiguration);
      }

      return Map<TTarget>(source, targetConfiguration);
    }

    /// <summary>
    /// Maps source to target type.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="targetConfiguration"></param>
    /// <returns></returns>
    public static TTarget Map<TTarget>(object source, TargetConfiguration targetConfiguration = null)
    {
      if (source == null)
      {
        return default(TTarget);
      }

      Configuration configuration = Configuration.Current;
      Type targetType = typeof(TTarget);
      IObjectMapper objectMapper = new ObjectMapper(configuration);

      if (targetConfiguration == null)
      {
        configuration.TargetConfigurations.TryGetValue(targetType, out targetConfiguration);
      }

      return (TTarget)objectMapper.Map(targetType, source, targetConfiguration);
    }

    public static IEnumerable<TTarget> MapAll<TTarget>(IEnumerable source, TargetConfiguration targetConfiguration = null)
    {
      foreach (object obj in source)
      {
        yield return Map<TTarget>(obj, targetConfiguration);
      }
    }

    private readonly Configuration _configuration;
  }
}