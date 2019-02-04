using BenchmarkDotNet.Attributes;

namespace FastMapper.Benchmark
{
  public class ObjectMapperBenchmarks
  {
    [GlobalSetup]
    public void Setup()
    {
      _sourceClass = new SourceClass
      {
        Age = 1,
        FirstName = "foo",
        NoOfDependants = 3,
        Title = "bar",
      };
      _sourceDynamic = new
      {
        Age = 1,
        FirstName = "foo",
        NoOfDependants = 3,
        Title = "bar",
      };
      _sourceStringDynamic = new
      {
        Age = "1",
        FirstName = "foo",
        NoOfDependants = "3",
        Title = "bar",
      };
    }

    [Benchmark]
    public void Map_classes()
    {
      ObjectMapper.Map<TargetClass>(_sourceClass);
    }

    [Benchmark]
    public void Map_dynamic()
    {
      ObjectMapper.Map<TargetClass>(_sourceDynamic);
    }

    [Benchmark]
    public void Map_dynamic_from_string()
    {
      ObjectMapper.Map<TargetClass>(_sourceStringDynamic);
    }

    [Benchmark]
    public void Map_all_from_dynamic()
    {
      ObjectMapper.MapAll<TargetClass>(new[] { _sourceStringDynamic, _sourceStringDynamic, _sourceStringDynamic, _sourceStringDynamic, _sourceStringDynamic });
    }

    private SourceClass _sourceClass;

    private dynamic _sourceDynamic;

    private dynamic _sourceStringDynamic;

    public class SourceClass
    {
      public string Title { get; set; }

      public string FirstName { get; set; }

      public int Age { get; set; }

      public int? NoOfDependants { get; set; }
    }

    public class TargetClass
    {
      public string Title { get; set; }

      public string FirstName { get; set; }

      public int Age { get; set; }

      public int? NoOfDependants { get; set; }
    }
  }
}