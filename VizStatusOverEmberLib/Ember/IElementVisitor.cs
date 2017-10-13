namespace VizStatusOverEmberLib.Ember
{
    public interface IElementVisitor<in TState, out TResult>
    {
        TResult Visit(Node element, TState state);

        TResult Visit(IntegerParameter element, TState state);

        TResult Visit(StringParameter element, TState state);

        TResult Visit(BooleanParameter element, TState state);
    }
}
