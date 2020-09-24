using System;
using System.Collections.Generic;

namespace FastMapper
{
  public interface ITypeAccessor
  {
    object CreateInstance();

    IEnumerable<IMember> GetMembers();

    void SetValue(object obj, string name, object value);

    object GetValue(object obj, string name);
  }
}