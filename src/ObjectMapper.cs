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

    /// <summary>
    /// Maps the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to map</param>
    /// <param name="source">The source to map from</param>
    /// <param name="targetConfiguration">Configuration to use</param>
    /// <param name="valueProvider">Value provider to use</param>
    /// <param name="declaringMember">The declaring member if this is type is being mapped from a declaring member</param>
    /// <returns></returns>
    public object Map(Type type, object source, TargetConfiguration targetConfiguration = null, ValueProvider valueProvider = null, IMember declaringMember = null)
    {
      if (source == null)
      {
        return null;
      }

      ITypeAccessor targetAccessor = TypeAccessorFactory.Create(type);
      object result = targetAccessor.CreateInstance();

      // get config for the target type if not provided
      if (targetConfiguration == null)
      {
        _configuration.TargetConfigurations.TryGetValue(type, out targetConfiguration);
      }

      // get the value provider if not provided
      if (valueProvider == null)
      {
        valueProvider = _configuration.ValueProviders.First(x => x.CanRead(source));
      }

      ValueBinderContext valueBinderContext = new ValueBinderContext(source, result, targetConfiguration, valueProvider, this);

      foreach (IMember targetMember in targetAccessor.GetMembers())
      {
        // recursion protection - if we are calling this bind from a member and attempt to bind to a target with the same type, continue to next target member
        if (declaringMember != null && targetMember.Type == declaringMember.DeclaringType)
        {
          continue;
        }

        ValueBinder valueBinder = _configuration.ValueBinders.First(x => x.CanBind(targetMember.Type));
        valueBinder.Bind(targetAccessor, targetMember, valueBinderContext);
      }

      return result;
    }

    /// <summary>
    /// Maps all the given <paramref name="type"/>s from the <paramref name="sources"/>.
    /// </summary>
    /// <param name="type">The type to map</param>
    /// <param name="sources">The source to map from</param>
    /// <param name="targetConfiguration">Configuration to use</param>
    /// <param name="declaringMember">The declaring member if this is type is being mapped from a declaring member</param>
    /// <returns></returns>
    public IEnumerable MapAll(Type type, IEnumerable sources, TargetConfiguration targetConfiguration = null, IMember declaringMember = null)
    {
      if (sources == null)
      {
        yield return null;
      }

      foreach (object obj in sources)
      {
        yield return Map(type, obj, targetConfiguration, declaringMember: declaringMember);
      }
    }

    /// <summary>
    /// Initialisation routine with callback contains the <see cref="Configuration"/>.
    /// </summary>
    /// <param name="action"></param>
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
            TypeAccessorFactory.Create(typeResolver.ResolvedType);
          }

          // preload the target types
          TypeAccessorFactory.Create(targetType);
        }
      }
    }

    /// <summary>
    /// Map with optional configuration.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="source"></param>
    /// <param name="targetConfigurationFactory"></param>
    /// <returns></returns>
    public static TTarget Map<TSource, TTarget>(TSource source, Action<TargetConfiguration<TSource, TTarget>> targetConfigurationFactory = null)
    {
      if (source == null)
      {
        return default;
      }

      TargetConfiguration<TSource, TTarget> targetConfiguration = GetTargetConfiguration(targetConfigurationFactory);

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
        return default;
      }

      Configuration configuration = Configuration.Current;
      Type targetType = typeof(TTarget);
      IObjectMapper objectMapper = new ObjectMapper(configuration);

      return (TTarget)objectMapper.Map(targetType, source, targetConfiguration);
    }

    /// <summary>
    /// Maps all sources to target type.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="sources"></param>
    /// <param name="targetConfiguration"></param>
    /// <returns></returns>
    public static IEnumerable<TTarget> MapAll<TTarget>(IEnumerable sources, TargetConfiguration targetConfiguration = null)
    {
      foreach (object source in sources)
      {
        yield return Map<TTarget>(source, targetConfiguration);
      }
    }

    /// <summary>
    /// Maps all sources to target type.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="sources"></param>
    /// <param name="targetConfigurationFactory"></param>
    /// <returns></returns>
    public static IEnumerable<TTarget> MapAll<TSource, TTarget>(IEnumerable sources, Action<TargetConfiguration<TSource, TTarget>> targetConfigurationFactory = null)
    {
      if (sources == null)
      {
        return Enumerable.Empty<TTarget>();
      }

      TargetConfiguration<TSource, TTarget> targetConfiguration = GetTargetConfiguration(targetConfigurationFactory);

      return MapAll<TTarget>(sources, targetConfiguration);
    }

    /// <summary>
    /// If a factory is passed will look for existing configurations or create a new one then invoke.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="targetConfigurationFactory"></param>
    /// <returns></returns>
    private static TargetConfiguration<TSource, TTarget> GetTargetConfiguration<TSource, TTarget>(Action<TargetConfiguration<TSource, TTarget>> targetConfigurationFactory = null)
    {
      TargetConfiguration<TSource, TTarget> targetConfiguration = null;

      if (targetConfigurationFactory != null)
      {
        Type targetType = typeof(TTarget);
        Configuration configuration = Configuration.Current;

        if (configuration.TargetConfigurations.TryGetValue(targetType, out TargetConfiguration existingConfiguration))
        {
          targetConfiguration = new TargetConfiguration<TSource, TTarget>(existingConfiguration);
        }
        else
        {
          targetConfiguration = new TargetConfiguration<TSource, TTarget>();
          configuration.TargetConfigurations[targetType] = targetConfiguration;
        }

        targetConfigurationFactory(targetConfiguration);
      }

      return targetConfiguration;
    }

    private readonly Configuration _configuration;
  }
}