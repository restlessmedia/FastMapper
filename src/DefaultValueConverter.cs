using System;

namespace FastMapper
{
  public class DefaultValueConverter : ValueConverter<object>
  {
    public override bool CanConvert(Type type)
    {
      return true;
    }

    public override object Convert(object value, Type destinationType)
    {
      using (new Profiler($"Convert to {destinationType.Name}"))
      {
        if (value != null)
        {
          Type sourceType = GetNullableTypeOrType(value.GetType());
          destinationType = GetNullableTypeOrType(destinationType);

          if (sourceType != destinationType && value is IConvertible)
          {
            using (new Profiler($"ChangeType {destinationType.Name}"))
            {
              value = System.Convert.ChangeType(value, destinationType);
            }
          }
        }

        return value;
      }
    }
  }
}