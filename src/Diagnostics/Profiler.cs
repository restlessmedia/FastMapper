using System;
using System.Diagnostics;

namespace System
{
  internal class Profiler : IDisposable
  {
    public Profiler(string name)
    {
      _name = name;
      _watch = Stopwatch.StartNew();
    }

    public double Elapsed
    {
      get
      {
        return _watch.Elapsed.TotalMilliseconds;
      }
    }

    public void Dispose()
    {
      _watch.Stop();

      if (Elapsed > MinThreshold)
      {
        Trace.TraceInformation($"{_name} took {Elapsed}ms.");
      }
    }

    public static void All(string name, int times, Action action)
    {
      using (new Profiler($"{name} ({times})"))
      {
        for (int i = 0; i < times; i++)
        {
          using (new Profiler($"{name} ({i}/{times})"))
          {
            action();
          }
        }
      }
    }

    public static double MinThreshold = 0.2;

    private readonly string _name;

    private readonly Stopwatch _watch;
  }
}