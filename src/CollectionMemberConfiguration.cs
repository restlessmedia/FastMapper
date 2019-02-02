using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace FastMapper
{
  public class CollectionMemberConfiguration<TTarget, TMember> : MemberConfiguration<TTarget, TMember>
    where TMember : IEnumerable
  {
    public CollectionMemberConfiguration(Expression<Func<TTarget, TMember>> expression)
      : base(expression) { }

    public override TypeResolver<TMember, TResolvedWith> ResolveWith<TResolvedWith>()
    {
      Type enumeratedType = typeof(TMember).GenericTypeArguments.First();
      TypeResolver<TMember, TResolvedWith> typeResolver = new TypeResolver<TMember, TResolvedWith>(enumeratedType, typeof(TResolvedWith));
      TypeResolvers.Add(typeResolver);
      return typeResolver;
    }
  }
}