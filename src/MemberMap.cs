using System.Reflection;

namespace FastMapper
{
  internal struct MemberMap
  {
    public MemberMap(string sourceMemberName, string targetMemberName)
    {
      SourceMemberName = sourceMemberName;
      TargetMemberName = targetMemberName;
    }

    public readonly string SourceMemberName;

    public readonly string TargetMemberName;
  }
}