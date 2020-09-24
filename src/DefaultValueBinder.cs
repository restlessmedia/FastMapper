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

    public override void Bind(ITypeAccessor targetAccessor, IMember targetMember, ValueBinderContext valueBinderContext)
    {
      object value;

      if (TryFindResolvedType(targetMember.Type, valueBinderContext.TargetConfiguration, out Type resolvedType))
      {
        value = valueBinderContext.ObjectMapper.Map(resolvedType, valueBinderContext.Source, valueProvider: valueBinderContext.ValueProvider, member: targetMember);
      }
      else
      {
        string name = GetSourceNameOrDefault(targetMember, valueBinderContext.TargetConfiguration);

        if (valueBinderContext.ValueProvider.TryGetValue(valueBinderContext.Source, name, out value) && value != null)
        {
          ValueConverter valueConverter = Configuration.ValueConverters.First(x => x.CanConvert(targetMember.Type));
          value = valueConverter.Convert(value, targetMember.Type);
        }
        else if (IsMappableType(targetMember.Type))
        {
          value = valueBinderContext.ObjectMapper.Map(targetMember.Type, valueBinderContext.Source, valueProvider: valueBinderContext.ValueProvider, member: targetMember);
        }
        else
        {
          return;
        }
      }

      targetAccessor.SetValue(valueBinderContext.Result, targetMember, value);
    }
  }
}