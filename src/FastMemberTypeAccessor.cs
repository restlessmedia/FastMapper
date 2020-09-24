using FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FastMapper
{
  internal class FastMemberTypeAccessor : ITypeAccessor
  {
    public FastMemberTypeAccessor(Type type)
    {
      _type = type;
      _typeAccessor = TypeAccessor.Create(type);
    }

    public object CreateInstance()
    {
      return _typeAccessor.CreateNew();
    }

    public IEnumerable<IMember> GetMembers()
    {
      return _typeAccessor.GetMembers().Select(member => new FastMapperMember(_type, member));
    }

    public object GetValue(object obj, string name)
    {
      return _typeAccessor[obj, name];
    }

    public void SetValue(object obj, string name, object value)
    {
      try
      {
        _typeAccessor[obj, name] = value;
      }
      catch
      {
        _MemberInfo member = _type.GetMember(name).FirstOrDefault();

        if (member != null)
        {
          try
          {
            switch (member.MemberType)
            {
              case MemberTypes.Field:
                {
                  ((FieldInfo)member).SetValue(obj, value);
                  break;
                }
              case MemberTypes.Property:
                {
                  ((PropertyInfo)member).SetValue(obj, value);
                  break;
                }
            }
          }
          catch
          {
            return;
          }
        }
      }
    }

    private class FastMapperMember : IMember
    {
      public FastMapperMember(Type declaringType, Member member)
      {
        DeclaringType = declaringType;
        _member = member;
      }

      public string Name => _member.Name;

      public Type Type => _member.Type;

      public bool CanWrite
      {
        get
        {
          try
          {
            return _member.CanWrite;
          }
          catch (NotSupportedException)
          {
            // fastmember throws invalid for fields but they can be written to.
            // here we'll check the underlying type to find out if it can be written to.
            MemberInfo memberInfo = DeclaringType.GetMember(Name).FirstOrDefault();

            if (memberInfo != null)
            {
              switch (memberInfo.MemberType)
              {
                case MemberTypes.Field:
                  {
                    return !((FieldInfo)memberInfo).IsInitOnly;
                  }
                case MemberTypes.Property:
                  {
                    return !((PropertyInfo)memberInfo).CanWrite;
                  }
              }
            }

            return false;
          }
        }
      }

      public bool CanRead => _member.CanRead;

      public Type DeclaringType { get; private set; }

      private readonly Member _member;
    }

    private readonly Type _type;

    private readonly TypeAccessor _typeAccessor;
  }
}
