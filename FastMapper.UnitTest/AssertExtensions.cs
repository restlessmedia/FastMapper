using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FastMapper.UnitTest
{
  public static class AssertExtensions
  {
    public static void MustEqual<T>(this T actual, T expected)
    {
      Assert.AreEqual(expected, actual);
    }

    public static void MustBeNull(this object actual)
    {
      MustEqual(actual, null);
    }

    public static void MustBeTrue(this bool actual)
    {
      MustEqual(true, actual);
    }

    public static void MustBeFalse(this bool actual)
    {
      MustEqual(false, actual);
    }

    public static void Throws<T>(this Action action)
      where T : Exception
    {
      Exception exception = null;

      try
      {
        action();
      }
      catch (Exception e)
      {
        exception = e;
      }

      MustBeTrue(exception != null && exception is T);
    }

    public static void Throws(this Action action)
    {
      Throws<Exception>(action);
    }

    public static void DoesNotThrow<T>(this Action action)
      where T : Exception
    {
      Exception exception = null;

      try
      {
        action();
      }
      catch (Exception e)
      {
        exception = e;
      }

      MustBeTrue(exception == null || !(exception is T));
    }

    public static void DoesNotThrow(this Action action)
    {
      DoesNotThrow<Exception>(action);
    }
  }
}