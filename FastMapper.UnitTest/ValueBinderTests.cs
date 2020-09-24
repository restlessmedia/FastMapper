using System;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastMapper.UnitTest
{
  public abstract class ValueBinderTests
  {
    [TestClass]
    public class Tests
    {
      [TestMethod]
      public void bind_handles_null_configuration()
      {
        Configuration configuration = new Configuration();
        TestBinder binder = new TestBinder(configuration);
        ITypeAccessor typeAccessor = TypeAccessorFactory.Create(typeof(Test));
        IMember member = typeAccessor.GetMembers().First();

        binder.Bind(typeAccessor, member, CreateContext(null));
      }

      public class Test
      {
        public string Foo { get; set; }
      }

      public class TestBinder : ValueBinder
      {
        public TestBinder(Configuration configuration)
          : base(configuration)
        {
        }

        public override void Bind(ITypeAccessor targetAccessor, IMember targetMember, ValueBinderContext valueBinderContext)
        {
          GetSourceNameOrDefault(targetMember, valueBinderContext.TargetConfiguration);
        }

        public override bool CanBind(Type targetMemberType)
        {
          return true;
        }
      }
    }

    public static ValueBinderContext CreateContext()
    {
      return CreateContext(new { }, new { });
    }

    public static ValueBinderContext CreateContext(TargetConfiguration targetConfiguration)
    {
      return CreateContext(new { }, new { }, targetConfiguration);
    }

    public static ValueBinderContext CreateContext(object source, object result)
    {
      return CreateContext(source, result, A.Fake<TargetConfiguration>());
    }

    public static ValueBinderContext CreateContext(object source, object result, TargetConfiguration targetConfiguration)
    {
      return new ValueBinderContext(source, result, targetConfiguration, A.Fake<ValueProvider>(), A.Fake<IObjectMapper>());
    }
  }
}