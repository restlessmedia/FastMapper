using System.Collections.Generic;

namespace FastMapper
{
  public class DictionaryValueProvider : ValueProvider<IDictionary<string, object>>
  {
    public override bool TryGetValue(object source, string name, out object value)
    {
      IDictionary<string, object> dictionary = (IDictionary<string, object>)source;

      if (dictionary.ContainsKey(name))
      {
        value = dictionary[name];
      }
      else
      {
        value = null;
      }

      return value != null;
    }
  }
}