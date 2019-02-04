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
      if (value != null)
      {
        Type sourceType = GetNullableTypeOrType(value.GetType());
        destinationType = GetNullableTypeOrType(destinationType);

        if (sourceType != destinationType && value is IConvertible)
        {
          value = System.Convert.ChangeType(value, destinationType);
        }
      }

      return value;
    }
  }
}