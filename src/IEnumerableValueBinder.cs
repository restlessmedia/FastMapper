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
      // additional check for IsGenericType prevents returning true for string which is ienumerable<char>
      return (targetMemberType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(targetMemberType)) || targetMemberType.IsArray;
    }

    public override void Bind(TypeAccessor targetAccessor, Member targetMember, ValueBinderContext valueBinderContext)
    {
      IEnumerable sourceEnumerable = FindSourceEnumerable(valueBinderContext.Source, targetMember, valueBinderContext.TargetConfiguration, valueBinderContext.ValueProvider);
      Type enumerableType = FindEnumerableType(targetMember, sourceEnumerable, valueBinderContext.TargetConfiguration);

      // if we don't have a type or the type is primative - we can't build an array list of it (prevents error when attempting to map to List<string> and we try and map to string and iterate through it's members)
      if (enumerableType == null || !IsMappableType(enumerableType))
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
      Type enumerableType = null;

      if (sourceEnumerable != null)
      {
        // we have a source enumerable to loop from
        enumerableType = (sourceEnumerable).GetType().GenericTypeArguments.FirstOrDefault();
      }
      else if (targetMember.Type.IsGenericType)
      {
        // the type is generic so get the first generic type i.e. IEnumerable<First>s
        enumerableType = targetMember.Type.GenericTypeArguments.FirstOrDefault();
      }
      else if (targetMember.Type.IsArray)
      {
        // we have an array, get the array type i.e ArrayType[]
        enumerableType = targetMember.Type.GetElementType();
      }

      if (enumerableType != null)
      {
        // attempt to find a type to use
        if (TryFindResolvedType(enumerableType, targetConfiguration, out Type resolvedType))
        {
          return resolvedType;
        }
        // we can't map to an abstract type we don't know how to map to
        else if (!enumerableType.IsAbstract)
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