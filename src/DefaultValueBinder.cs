using FastMember;
using System;
using System.Linq;

namespace FastMapper
{
  public class DefaultValueBinder : ValueBinder
  {
    public DefaultValueBinder(Configuration configuration)
      : base(configuration) { }

    public override bool CanBind(Type targetMemberType)
    {
      return true;
    }

    public override void Bind(TypeAccessor targetAccessor, Member targetMember, ValueBinderContext valueBinderContext)
    {
      if (TryFindResolvedType(targetMember.Type, valueBinderContext.TargetConfiguration, out Type resolvedType))
      {
        targetAccessor[valueBinderContext.Result, targetMember.Name] = valueBinderContext.ObjectMapper.Map(resolvedType, valueBinderContext.Source, valueBinderContext.TargetConfiguration);
      }
      else
      {
        string name = GetSourceNameOrDefault(targetMember, valueBinderContext.TargetConfiguration);

        if (valueBinderContext.ValueProvider.TryGetValue(valueBinderContext.Source, name, out object value) && value != null)
        {
          ValueConverter valueConverter = Configuration.ValueConverters.First(x => x.CanConvert(targetMember.Type));
          value = valueConverter.Convert(value, targetMember.Type);
          targetAccessor[valueBinderContext.Result, targetMember.Name] = value;
        }
        else if (CanMap(targetMember.Type))
        {
          targetAccessor[valueBinderContext.Result, targetMember.Name] = valueBinderContext.ObjectMapper.Map(targetMember.Type, valueBinderContext.Source);
        }
      }
    }

    /// <summary>
    /// Returns true if we can map to the specific type i.e it's not a string or primative type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool CanMap(Type type)
    {
      if (type.IsPrimitive)
      {
        return false;
      }

      if(type == typeof(string))
      {
        return false;
      }

      return type.IsClass;
    }
  }
}