namespace VizStatusOverEmberLib.Ember
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class IntegerParameter : Parameter<long>
    {
        public IntegerParameter(int number, Element parent, string identifier, Dispatcher dispatcher, int min, int max, bool isWriteable)
        : base(number, parent, identifier, dispatcher, isWriteable)
        {
            Minimum = min;
            Maximum = max;
        }

        public long Minimum { get; }

        public long Maximum { get; }

        public override TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state)
        {
            return visitor.Visit(this, state);
        }
    }
}
