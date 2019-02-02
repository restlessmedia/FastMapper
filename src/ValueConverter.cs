using System;

namespace FastMapper
{
  public abstract class ValueConverter<T> : ValueConverter
  {
    public override bool CanConvert(Type type)
    {
      return typeof(T) == type;
    }
  }

  public abstract class ValueConverter
  {
    public abstract bool CanConvert(Type type);

    public abstract object Convert(object value, Type type);

    /// <summary>
    /// Returns the underlying type if the type is nullable or just the type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetNullableTypeOrType(Type type)
    {
      if (!type.IsGenericType)
      {
        return type;
      }

      return Nullable.GetUnderlyingType(type) ?? type;
    }
  }
}