using FastMember;
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
      TypeAccessor sourceAccessor = TypeAccessor.Create(source.GetType());
      MemberSet sourceMembers = sourceAccessor.GetMembers();
      Member sourceMember = sourceMembers.FirstOrDefault(x => x.CanRead && string.Equals(x.Name, name));
      value = null;

      if (sourceMember == null)
      {
        return false;
      }

      value = sourceAccessor[source, name];

      return true;
    }
  }
}