using System;
using System.Collections;
using System.Collections.Generic;

namespace FastMapper
{
  public interface IObjectMapper
  {
    object Map(Type type, object source, TargetConfiguration targetConfiguration = null);

    object Map(Type type, object source, ValueProvider valueProvider, TargetConfiguration targetConfiguration);

    IEnumerable MapAll(Type type, IEnumerable source, TargetConfiguration targetConfiguration = null);
  }
}