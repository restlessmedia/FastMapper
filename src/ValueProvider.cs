namespace FastMapper
{
  public abstract class ValueProvider<T>: ValueProvider
  {
    public override bool CanRead(object source)
    {
      return source is T;
    }
  }

  public abstract class ValueProvider
  {
    public abstract bool CanRead(object source);

    public abstract bool TryGetValue(object source, string name, out object value);
  }
}