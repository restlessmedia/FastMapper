using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastMapper.UnitTest
{
  [TestClass]
  public class TargetConfigurationTests
  {
    [TestMethod]
    public void ForEach_throws_exception_when_not_supported_ienumerable()
    {
      Action action = () => new TargetConfiguration<TestClass>().ForEach(x => x.Enumerable);

      action.Throws();
    }

    [TestMethod]
    public void ForEach_does_not_throw_exception_when_supported_ienumerable()
    {
      Action action = () => new TargetConfiguration<TestClass>().ForEach(x => x.TypedEnumerable);

      action.DoesNotThrow();
    }

    private class TestClass
    {
      public IEnumerable Enumerable;

      public IEnumerable<DateTime> TypedEnumerable;
    }
  }
}
