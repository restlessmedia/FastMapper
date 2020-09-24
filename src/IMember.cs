using System;

namespace FastMapper
{
  public interface IMember
  {
    string Name { get; }

    Type Type { get; }

    Type DeclaringType { get; }

    bool CanWrite { get; }

    bool CanRead { get; }
  }
}
