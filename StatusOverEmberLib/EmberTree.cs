namespace VizStatusOverEmberLib
{
    using System.Linq;
    using Ember;

    public static class EmberTree
    {
        public static Node Create(Dispatcher dispatcher)
        {
            var root = Node.CreateRoot();
            var vizEngine = new Node(1, root, "vizengine");
            CreateInputsParentNode(vizEngine, 1, dispatcher);
            return root;
        }

        public static bool SetParameter<T>(Dispatcher dispatcher, string path, T value)
        {
            return SetParameter(dispatcher?.Root, path, value);
        }

        private static void CreateInputsParentNode(Element parent, int nodeNumber, Dispatcher dispatcher)
        {
            var inputs = new Node(nodeNumber, parent, "inputs") { Description = "The inputs to the Viz Engine" };

            for (var i = 0; i < 4; i++)
            {
                var input = new Node(i + 1, inputs, $"input{i + 1}");

                // ReSharper disable once ObjectCreationAsStatement
                new BooleanParameter(1, input, "tally", dispatcher, false)
                { Value = false };
            }
        }

        private static bool SetParameter<T>(Element node, string path, T value)
        {
            var parts = path.Split('/');
            var element = node;
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }

                var child = element?.Children.SingleOrDefault(c => c.Identifier == part);
                if (child == null)
                {
                    return false;
                }

                element = child;
            }

            if (element is Parameter<T> cast)
            {
                cast.Value = value;
                return true;
            }

            return false;
        }
    }
}