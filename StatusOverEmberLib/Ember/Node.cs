namespace VizStatusOverEmberLib.Ember
{
    using System.Collections.Generic;

    public class Node : Element
    {
        private readonly List<Element> _children = new List<Element>();

        public Node(int number, Element parent, string identifier)
        : base(number, parent, identifier)
        {
        }

        public override IEnumerable<Element> Children => _children;

        public override int ChildrenCount => _children.Count;

        public static Node CreateRoot()
        {
            return new Node(0, null, null);
        }

        public override void AddChild(Element child)
        {
            _children.Add(child);
        }

        public override TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state)
        {
            return visitor.Visit(this, state);
        }
    }
}
