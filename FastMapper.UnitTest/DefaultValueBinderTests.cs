using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FastMapper.UnitTest
{
  [TestClass]
  public class DefaultValueBinderTests : ValueBinderTests
  {
    [TestMethod]
    public void bind_does_not_overwrite_with_null_when_no_source_value_found()
    {
      ITypeAccessor typeAccessor = TypeAccessorFactory.Create(typeof(Test));
      object source = new { };
      Test target = new Test
      {
        Foo = "foo"
      };
      IMember member = typeAccessor.GetMembers().First();
      ValueBinderContext valueBinderContext = CreateContext(source, target);
      new DefaultValueBinder(new Configuration()).Bind(typeAccessor, member, valueBinderContext);

      // check that the binder doesn't write null when it doesn't find a source value
      typeAccessor.GetValue(target, member).MustEqual(target.Foo);
    }

    private class Test
    {
      public string Foo { get; set; }
    }
  }
}