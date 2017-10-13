namespace VizStatusOverEmberLib.Ember
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Element
    {
        private readonly object _sync = new object();
        private string _identifierPath;
        private int[] _path;

        protected Element(int number, Element parent, string identifier)
        {
            Parent = parent;
            Number = number;
            Identifier = identifier;

            parent?.AddChild(this);
        }

        public Element Parent { get; }

        public int Number { get; }

        public string Identifier { get; }

        public string SchemaIdentifier { get; set; }

        public virtual int ChildrenCount => 0;

        public virtual IEnumerable<Element> Children
        {
            get { yield break; }
        }

        public string Description { get; set; }

        public bool IsRoot => Parent == null;

        public string IdentifierPath
        {
            get
            {
                lock (_sync)
                {
                    if (_identifierPath != null)
                    {
                        return _identifierPath;
                    }

                    var list = new LinkedList<string>();
                    list.AddFirst(Identifier);

                    var element = Parent;

                    while (!element.IsRoot)
                    {
                        list.AddFirst(element.Identifier);
                        element = element.Parent;
                    }

                    _identifierPath = string.Join("/", list);
                }

                return _identifierPath;
            }
        }

        public int[] Path
        {
            get
            {
                lock (_sync)
                {
                    if (_path == null)
                    {
                        var path = new LinkedList<int>();
                        var elem = this;

                        while (elem.IsRoot == false)
                        {
                            path.AddFirst(elem.Number);
                            elem = elem.Parent;
                        }

                        _path = path.ToArray();
                    }
                }

                return _path;
            }
        }

        protected object SyncRoot => _sync;

        public virtual void AddChild(Element child)
        {
        }

        public Element ResolveChild(int[] path, out IDynamicPathHandler dynamicPathHandler)
        {
            var element = this;

            dynamicPathHandler = null;

            foreach (var number in path)
            {
                var child = (from elem in element.Children
                             where elem.Number == number
                             select elem)
                             .FirstOrDefault();

                if (child == null)
                {
                    var handler = element as IDynamicPathHandler;
                    if (handler != null)
                    {
                        dynamicPathHandler = handler;
                    }

                    return null;
                }

                element = child;
            }

            return element;
        }

        public abstract TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state);
    }
}
