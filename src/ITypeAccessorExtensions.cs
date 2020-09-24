namespace FastMapper
{
  public static class ITypeAccessorExtensions
  {
    /// <summary>
    /// Returns the value for the member.
    /// </summary>
    /// <param name="typeAccessor"></param>
    /// <param name="obj"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static object GetValue(this ITypeAccessor typeAccessor, object obj, IMember member)
    {
      return typeAccessor.GetValue(obj, member.Name);
    }

    public static void SetValue(this ITypeAccessor typeAccessor, object obj, IMember member, object value)
    {
      typeAccessor.SetValue(obj, member.Name, value);
    }
  }
}
