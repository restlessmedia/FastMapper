using System;
using System.Collections.Generic;
using System.Linq;
using FastMember;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastMapper.UnitTest
{
  [TestClass]
  public class IEnumerableValueBinderTests : ValueBinderTests
  {
    [TestMethod]
    public void CanBind_returns_true_ienumerable()
    {
      IEnumerableValueBinder valueBinder = new IEnumerableValueBinder(new Configuration());
      valueBinder.CanBind(typeof(IEnumerable<DateTime>)).MustBeTrue();
    }

    /// <summary>
    /// This is to prevent string being iterated over as it implements IEnumerable<char> and we want to ignore.
    /// </summary>
    [TestMethod]
    public void CanBind_returns_false_when_string()
    {
      IEnumerableValueBinder valueBinder = new IEnumerableValueBinder(new Configuration());
      valueBinder.CanBind(typeof(string)).MustBeFalse();
    }

    [TestMethod]
    public void does_not_bind_when_concrete_enumerable_type_cannot_be_found()
    {
      IEnumerableValueBinder valueBinder = new IEnumerableValueBinder(new Configuration());
      TypeAccessor typeAccessor = TypeAccessor.Create(typeof(Test));
      Member member = typeAccessor.GetMembers().First();
      object source = new { };
      Test target = new Test();
      ValueBinderContext valueBinderContext = CreateContext(source, target);

      valueBinder.Bind(typeAccessor, member, valueBinderContext);

      typeAccessor[target, member.Name].MustBeNull();
    }

    private class Test
    {
      public string Foo { get; set; }
    }
  }
}