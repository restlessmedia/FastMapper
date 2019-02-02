using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastMapper.UnitTest
{
  [TestClass]
  public class EnumValueConverterTests
  {
    [TestMethod]
    public void CanConvert_returns_true_when_nullable_enum()
    {
      new EnumValueConverter().CanConvert(typeof(Foo?)).MustBeTrue();
    }

    [TestMethod]
    public void Convert_works_with_nullable_enum()
    {
      new EnumValueConverter().Convert(1, typeof(Foo?)).MustEqual(Foo.Bar);
    }

    public enum Foo
    {
      None = 0,
      Bar = 1,
    }
  }
}
