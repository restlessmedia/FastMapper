namespace FastMapper
{
  public struct ValueBinderContext
  {
    public ValueBinderContext(object source, object result, TargetConfiguration targetConfiguration, ValueProvider valueProvider, IObjectMapper objectMapper)
    {
      Source = source;
      Result = result;
      TargetConfiguration = targetConfiguration;
      ValueProvider = valueProvider;
      ObjectMapper = objectMapper;
    }

    public readonly object Source;

    public readonly object Result;

    public readonly TargetConfiguration TargetConfiguration;

    public readonly ValueProvider ValueProvider;

    public readonly IObjectMapper ObjectMapper;
  }
}