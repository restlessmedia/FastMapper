using System;
using System.Collections.Generic;

namespace FastMapper
{
  public class Configuration
  {
    public Configuration()
    {
      TargetConfigurations = new Dictionary<Type, TargetConfiguration>();
      ValueProviders = new ValueProvider[]
      {
        new DictionaryValueProvider(),
        new DefaultValueProvider(),
      };
      ValueConverters = new ValueConverter[]
      {
        new EnumValueConverter(),
        new DefaultValueConverter(),
      };
      ValueBinders = new ValueBinder[]
      {
        new IEnumerableValueBinder(this),
        new DefaultValueBinder(this)
      };
    }

    public void Add<TTarget>(Action<TargetConfiguration<TTarget>> action)
    {
      TargetConfiguration<TTarget> configuration = new TargetConfiguration<TTarget>();
      action(configuration);
      TargetConfigurations.Add(typeof(TTarget), configuration);
    }

    /// <summary>
    /// Lazily loads the type maps which may incur an initial performance penalty on the first map.
    /// </summary>
    public bool LazyLoad = true;

    internal static Configuration Current
    {
      get
      {
        return _current.Value;
      }
    }

    internal readonly Dictionary<Type, TargetConfiguration> TargetConfigurations;

    internal readonly ValueProvider[] ValueProviders;

    internal readonly ValueConverter[] ValueConverters;

    internal readonly ValueBinder[] ValueBinders;

    private static Lazy<Configuration> _current = new Lazy<Configuration>(() => new Configuration());
  }
}