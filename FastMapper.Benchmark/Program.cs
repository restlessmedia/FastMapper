using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace FastMapper.Benchmark
{
  class Program
  {
    static void Main(string[] args)
    {
      Summary summary = BenchmarkRunner.Run<ObjectMapperBenchmarks>();
    }
  }
}