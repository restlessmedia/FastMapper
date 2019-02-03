using FastMember;
using System;
using System.Collections;
using System.Linq;

namespace FastMapper
{
  public class IEnumerableValueBinder : ValueBinder<IEnumerable>
  {
    public IEnumerableValueBinder(Configuration configuration)
      : base(configuration) { }

    public override bool CanBind(Type targetMemberType)
    {
      return targetMemberType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(targetMemberType);
    }

    public override void Bind(TypeAccessor targetAccessor, Member targetMember, ValueBinderContext valueBinderContext)
    {
      IEnumerable sourceEnumerable = FindSourceEnumerable(valueBinderContext.Source, targetMember, valueBinderContext.TargetConfiguration, valueBinderContext.ValueProvider);
      Type enumerableType = FindEnumerableType(targetMember, sourceEnumerable, valueBinderContext.TargetConfiguration);

      if (enumerableType == null)
      {
        return;
      }

      if (sourceEnumerable != null)
      {
        // we have a source enumerable we need to convert to target enumerable
        targetAccessor[valueBinderContext.Result, targetMember.Name] = valueBinderContext.ObjectMapper.MapAll(enumerableType, sourceEnumerable, valueBinderContext.TargetConfiguration);
      }
      else
      {
        // otherwise we create an array with a single entry where we've mapped from the original object and set the value to that
        object value = valueBinderContext.ObjectMapper.Map(enumerableType, valueBinderContext.Source, valueBinderContext.ValueProvider, valueBinderContext.TargetConfiguration);
        ArrayList list = new ArrayList { value };
        targetAccessor[valueBinderContext.Result, targetMember.Name] = list.ToArray(enumerableType);
      }
    }

    /// <summary>
    /// Finds the type of the enumerable which may be resolved from configuration.
    /// </summary>
    /// <param name="targetMember"></param>
    /// <param name="sourceEnumerable"></param>
    /// <param name="targetConfiguration"></param>
    /// <returns></returns>
    private static Type FindEnumerableType(Member targetMember, IEnumerable sourceEnumerable, TargetConfiguration targetConfiguration)
    {
      Type type;

      if (sourceEnumerable != null)
      {
        // we have a source enumerable to loop from
        type = (sourceEnumerable).GetType();
      }
      else
      {
        // we don't have a source enumerable to copy from, try to build up 1 target object from the source data
        type = targetMember.Type;
      }

      Type enumerableType = type.GenericTypeArguments.FirstOrDefault();

      if (enumerableType != null)
      {
        if (TryFindResolvedType(enumerableType, targetConfiguration, out Type resolvedType))
        {
          return resolvedType;
        }

        if (!enumerableType.IsAbstract)
        {
          return enumerableType;
        }
      }

      return null;
    }

    /// <summary>
    /// Finds the source enumerable member.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetMember"></param>
    /// <param name="targetConfiguration"></param>
    /// <param name="valueProvider"></param>
    /// <returns></returns>
    private static IEnumerable FindSourceEnumerable(object source, Member targetMember, TargetConfiguration targetConfiguration, ValueProvider valueProvider)
    {
      string name = GetSourceNameOrDefault(targetMember, targetConfiguration);

      if (valueProvider.TryGetValue(source, name, out object sourceValue) && sourceValue is IEnumerable enumerable)
      {
        return enumerable;
      }

      return null;
    }
  }
}