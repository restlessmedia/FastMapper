using System;
using System.Collections;

namespace FastMapper
{
  public interface IObjectMapper
  {
    object Map(Type type, object source, TargetConfiguration targetConfiguration = null, ValueProvider valueProvider = null, IMember declaringMember = null);

    IEnumerable MapAll(Type type, IEnumerable sources, TargetConfiguration targetConfiguration = null, IMember declaringMember = null); 
  }
}