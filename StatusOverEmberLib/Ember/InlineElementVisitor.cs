namespace VizStatusOverEmberLib.Ember
{
    using System;

    internal class InlineElementVisitor : IElementVisitor<object, object>
    {
        private readonly Action<Node> _onNode;
        private readonly Action<ParameterBase> _onParameter;

        public InlineElementVisitor(Action<Node> onNode = null, Action<ParameterBase> onParameter = null)
        {
            _onNode = onNode;
            _onParameter = onParameter;
        }

        object IElementVisitor<object, object>.Visit(Node element, object state)
        {
            _onNode?.Invoke(element);
            return null;
        }

        object IElementVisitor<object, object>.Visit(IntegerParameter element, object state)
        {
            _onParameter?.Invoke(element);
            return null;
        }

        object IElementVisitor<object, object>.Visit(BooleanParameter element, object state)
        {
            _onParameter?.Invoke(element);
            return null;
        }

        object IElementVisitor<object, object>.Visit(StringParameter element, object state)
        {
            _onParameter?.Invoke(element);
            return null;
        }
    }
}