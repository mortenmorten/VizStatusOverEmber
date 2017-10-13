namespace VizStatusOverEmberLib.Ember
{
    using System.Linq;
    using EmberLib.Glow;

    internal class Walker : GlowWalker
    {
        private readonly Dispatcher _dispatcher;
        private readonly Client _source;

        public Walker(Dispatcher dispatcher, Client source)
        {
            _dispatcher = dispatcher;
            _source = source;
        }

        protected override void OnCommand(GlowCommand glow, int[] path)
        {
            var parent = _dispatcher.Root.ResolveChild(path, out var dynamicPathHandler);

            if (parent != null)
            {
                if (glow.Number != GlowCommandType.GetDirectory)
                {
                    return;
                }

                var glowRoot = GlowRootElementCollection.CreateRoot();
                var options = new Dispatcher.ElementToGlowOptions { DirFieldMask = glow.DirFieldMask ?? GlowFieldFlags.All };

                var visitor = new InlineElementVisitor(
                    node =>
                    {
                        // "dir" in node
                        if (node.ChildrenCount == 0)
                        {
                            glowRoot.Insert(new GlowQualifiedNode(node.Path));
                        }
                        else
                        {
                            var glowChildren = from element in node.Children
                                               select _dispatcher.ElementToGlow(element, options);

                            foreach (var glowChild in glowChildren)
                            {
                                glowRoot.Insert(glowChild);
                            }
                        }
                    },
                    parameter =>
                    {
                        // "dir" in parameter
                        glowRoot.Insert(_dispatcher.ElementToGlow(parameter, options));
                    });

                parent.Accept(visitor, null); // run inline visitor against parent

                _source.Write(glowRoot);
            }
            else
            {
                dynamicPathHandler?.HandleCommand(glow, path, _source);
            }
        }

        protected override void OnParameter(GlowParameterBase glow, int[] path)
        {
            if (_dispatcher.Root.ResolveChild(path, out var dynamicPathHandler) is ParameterBase parameter)
            {
                var glowValue = glow.Value;

                if (glowValue == null)
                {
                    return;
                }

                switch (glowValue.Type)
                {
                    case GlowParameterType.Integer:
                        {
                            if (parameter is IntegerParameter integerParameter)
                            {
                                integerParameter.Value = glowValue.Integer;
                            }

                            break;
                        }

                    case GlowParameterType.String:
                        {
                            if (parameter is StringParameter stringParameter)
                            {
                                stringParameter.Value = glowValue.String;
                            }

                            break;
                        }

                    case GlowParameterType.Boolean:
                        {
                            if (parameter is BooleanParameter booleanParameter)
                            {
                                booleanParameter.Value = glowValue.Boolean;
                            }

                            break;
                        }
                }
            }
            else
            {
                dynamicPathHandler?.HandleParameter(glow, path, _source);
            }
        }

        protected override void OnMatrix(GlowMatrixBase glow, int[] path)
        {
        }

        protected override void OnNode(GlowNodeBase glow, int[] path)
        {
        }

        protected override void OnStreamEntry(GlowStreamEntry glow)
        {
        }

        protected override void OnFunction(GlowFunctionBase glow, int[] path)
        {
        }

        protected override void OnInvocationResult(GlowInvocationResult glow)
        {
        }

        protected override void OnTemplate(GlowTemplateBase glow, int[] path)
        {
        }
    }
}
