using System.Collections.Generic;
using System.Linq;

namespace FastMapper
{
  public class DefaultValueProvider : ValueProvider
  {
    public override bool CanRead(object source)
    {
      return true;
    }

    public override bool TryGetValue(object source, string name, out object value)
    {
      ITypeAccessor sourceAccessor = TypeAccessorFactory.Create(source.GetType());
      IEnumerable<IMember> sourceMembers = sourceAccessor.GetMembers();
      IMember sourceMember = sourceMembers.FirstOrDefault(x => x.CanRead && string.Equals(x.Name, name));
      value = null;

      if (sourceMember == null)
      {
        return false;
      }

      value = sourceAccessor.GetValue(source, name);

      return true;
    }
  }
}