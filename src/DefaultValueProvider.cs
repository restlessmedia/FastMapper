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
      // TODO: check why we can't call x.CanRead to only find source members we can read from
      Member sourceMember = sourceMembers.FirstOrDefault(x => x.Name == name);
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