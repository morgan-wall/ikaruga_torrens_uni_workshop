public interface IContextDefinition<TContext>
    where TContext : IContext
{
    TContext MakeContext();
}
