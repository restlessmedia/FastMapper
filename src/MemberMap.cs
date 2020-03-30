using System.Reflection;

namespace FastMapper
{
  internal struct MemberMap
  {
    public MemberMap(string sourceMemberName, MemberInfo member)
    {
      SourceMemberName = sourceMemberName;
      _member = member;
    }

    public readonly string SourceMemberName;

    public string TargetMemberName
    {
      get
      {
        return _member.Name;
      }
    }

    private readonly MemberInfo _member;
  }
}