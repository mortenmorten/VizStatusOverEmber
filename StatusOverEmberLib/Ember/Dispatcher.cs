namespace VizStatusOverEmberLib.Ember
{
    using System;
    using EmberLib.Glow;

    public sealed class Dispatcher : IElementVisitor<Dispatcher.ElementToGlowOptions, GlowContainer>
    {
        public event EventHandler<GlowRootReadyArgs> GlowRootReady;

        public Element Root { get; set; }

        public void DispatchGlow(GlowContainer glow, Client source)
        {
            var walker = new Walker(this, source);

            walker.Walk(glow);
        }

        public void NotifyParameterValueChanged(ParameterBase parameter)
        {
            var options = new ElementToGlowOptions
            {
                DirFieldMask = GlowFieldFlags.Value
            };

            var glowParam = ElementToGlow(parameter, options);

            var glow = GlowRootElementCollection.CreateRoot();
            glow.Insert(glowParam);

            OnGlowRootReady(new GlowRootReadyArgs(glow, null));
        }

        public void NotifyParameterValueChanged(int[] parameterPath, GlowValue value)
        {
            var glowParam = new GlowQualifiedParameter(parameterPath)
            {
                Value = value,
            };

            var glow = GlowRootElementCollection.CreateRoot();
            glow.Insert(glowParam);

            OnGlowRootReady(new GlowRootReadyArgs(glow, null));
        }

        public GlowContainer ElementToGlow(Element element, ElementToGlowOptions options)
        {
            return element.Accept(this, options);
        }

        GlowContainer IElementVisitor<ElementToGlowOptions, GlowContainer>.Visit(Node element, ElementToGlowOptions state)
        {
            var glow = new GlowQualifiedNode(element.Path);
            var dirFieldMask = state.DirFieldMask;

            if (dirFieldMask.HasBits(GlowFieldFlags.Identifier))
            {
                glow.Identifier = element.Identifier;
            }

            if (dirFieldMask.HasBits(GlowFieldFlags.Description)
                && string.IsNullOrEmpty(element.Description) == false)
            {
                glow.Description = element.Description;
            }

            if (dirFieldMask == GlowFieldFlags.All
                && string.IsNullOrEmpty(element.SchemaIdentifier) == false)
            {
                glow.SchemaIdentifiers = element.SchemaIdentifier;
            }

            return glow;
        }

        GlowContainer IElementVisitor<ElementToGlowOptions, GlowContainer>.Visit(IntegerParameter element, ElementToGlowOptions state)
        {
            var glow = new GlowQualifiedParameter(element.Path);
            var dirFieldMask = state.DirFieldMask;

            if (dirFieldMask.HasBits(GlowFieldFlags.Identifier))
            {
                glow.Identifier = element.Identifier;
            }

            if (dirFieldMask.HasBits(GlowFieldFlags.Description) && !string.IsNullOrEmpty(element.Description))
            {
                glow.Description = element.Description;
            }

            if (dirFieldMask.HasBits(GlowFieldFlags.Value))
            {
                glow.Value = new GlowValue(element.Value);
            }

            if (dirFieldMask == GlowFieldFlags.All)
            {
                glow.Minimum = new GlowMinMax(element.Minimum);
                glow.Maximum = new GlowMinMax(element.Maximum);

                if (element.IsWriteable)
                {
                    glow.Access = GlowAccess.ReadWrite;
                }
            }

            if (dirFieldMask == GlowFieldFlags.All && string.IsNullOrEmpty(element.SchemaIdentifier) == false)
            {
                glow.SchemaIdentifiers = element.SchemaIdentifier;
            }

            return glow;
        }

        GlowContainer IElementVisitor<ElementToGlowOptions, GlowContainer>.Visit(StringParameter element, ElementToGlowOptions state)
        {
            var glow = new GlowQualifiedParameter(element.Path);
            var dirFieldMask = state.DirFieldMask;

            if (dirFieldMask.HasBits(GlowFieldFlags.Identifier))
            {
                glow.Identifier = element.Identifier;
            }

            if (dirFieldMask.HasBits(GlowFieldFlags.Description) && !string.IsNullOrEmpty(element.Description))
            {
                glow.Description = element.Description;
            }

            if (dirFieldMask.HasBits(GlowFieldFlags.Value))
            {
                glow.Value = new GlowValue(element.Value);
            }

            if (dirFieldMask == GlowFieldFlags.All && element.IsWriteable)
            {
                glow.Access = GlowAccess.ReadWrite;
            }

            if (dirFieldMask == GlowFieldFlags.All && !string.IsNullOrEmpty(element.SchemaIdentifier))
            {
                glow.SchemaIdentifiers = element.SchemaIdentifier;
            }

            return glow;
        }

        GlowContainer IElementVisitor<ElementToGlowOptions, GlowContainer>.Visit(BooleanParameter element, ElementToGlowOptions state)
        {
            var glow = new GlowQualifiedParameter(element.Path);
            var dirFieldMask = state.DirFieldMask;

            if (dirFieldMask.HasBits(GlowFieldFlags.Identifier))
            {
                glow.Identifier = element.Identifier;
            }

            if (dirFieldMask.HasBits(GlowFieldFlags.Description) && !string.IsNullOrEmpty(element.Description))
            {
                glow.Description = element.Description;
            }

            if (dirFieldMask.HasBits(GlowFieldFlags.Value))
            {
                glow.Value = new GlowValue(element.Value);
            }

            if (dirFieldMask == GlowFieldFlags.All && element.IsWriteable)
            {
                glow.Access = GlowAccess.ReadWrite;
            }

            if (dirFieldMask == GlowFieldFlags.All && !string.IsNullOrEmpty(element.SchemaIdentifier))
            {
                glow.SchemaIdentifiers = element.SchemaIdentifier;
            }

            return glow;
        }

        private void OnGlowRootReady(GlowRootReadyArgs oArgs)
        {
            GlowRootReady?.Invoke(this, oArgs);
        }

        public class ElementToGlowOptions
        {
            public int DirFieldMask { get; set; }
        }
    }
}
