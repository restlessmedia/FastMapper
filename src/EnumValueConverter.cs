using System;

namespace FastMapper
{
  public class EnumValueConverter : ValueConverter
  {
    public override bool CanConvert(Type type)
    {
      return GetNullableTypeOrType(type).IsEnum;
    }

    public override object Convert(object value, Type type)
    {
      using (new Profiler($"Enum Convert for {type.Name}"))
      {
        if (value != null)
        {
          value = Enum.Parse(GetNullableTypeOrType(type), value.ToString());
        }

        return value;
      }
    }
  }
}
