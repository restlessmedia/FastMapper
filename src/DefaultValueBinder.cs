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
        using (new Profiler($"Bind {targetMember.Name} from resolved type map"))
        {
          targetAccessor[valueBinderContext.Result, targetMember.Name] = valueBinderContext.ObjectMapper.Map(resolvedType, valueBinderContext.Source, valueBinderContext.TargetConfiguration);
        }
      }
      else
      {
        using (new Profiler($"Bind {targetMember.Name} from source"))
        {
          string name = GetSourceNameOrDefault(targetMember, valueBinderContext.TargetConfiguration);

          if (valueBinderContext.ValueProvider.TryGetValue(valueBinderContext.Source, name, out object value) && value != null)
          {
            ValueConverter valueConverter = Configuration.ValueConverters.FirstOrDefault(x => x.CanConvert(targetMember.Type));
            value = valueConverter.Convert(value, targetMember.Type);
            targetAccessor[valueBinderContext.Result, targetMember.Name] = value;
          }
        }
      }
    }
  }
}