using System;

namespace FastMapper
{
  internal class TypeAccessorFactory
  {
    public static ITypeAccessor Create(Type type)
    {
      return new FastMemberTypeAccessor(type);
    }
  }
}